using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Reflection.Emit;
using CodeFirstProfiledEF.Models;

namespace Meracord.Data.SqlAuditor
{
    [System.ComponentModel.DesignerCategory("")]
    public class AuditedDbCommand : DbCommand, ICloneable
    {
        private DbCommand _command;
        private DbConnection _connection;
        private DbTransaction _transaction;
        private ISqlAuditor _auditor;

        private bool _bindByName;
        /// <summary>
        /// If the underlying command supports BindByName, this sets/clears the underlying
        /// implementation accordingly. This is required to support OracleCommand from dapper-dot-net
        /// </summary>
        public bool BindByName
        {
            get { return _bindByName; }
            set
            {
                if (_bindByName != value) {
                    if (_command != null) {
                        var inner = GetBindByName(_command.GetType());
                        if (inner != null) inner(_command, value);
                    }
                    _bindByName = value;
                }
            }
        }

        private static Link<Type, Action<IDbCommand, bool>> _bindByNameCache;
        static Action<IDbCommand, bool> GetBindByName(Type commandType)
        {
            if (commandType == null) return null; // Garbage In/Garbage Out
            Action<IDbCommand, bool> action;
            if (Link<Type, Action<IDbCommand, bool>>.TryGet(_bindByNameCache, commandType, out action)) {
                return action;
            }
            var prop = commandType.GetProperty("BindByName", BindingFlags.Public | BindingFlags.Instance);
            action = null;
            ParameterInfo[] indexers;
            MethodInfo setter;
            if (prop != null && prop.CanWrite && prop.PropertyType == typeof(bool)
                && ((indexers = prop.GetIndexParameters()) == null || indexers.Length == 0)
                && (setter = prop.GetSetMethod()) != null
                ) {
                var method = new DynamicMethod(commandType.Name + "_BindByName", null, new[] { typeof(IDbCommand), typeof(bool) });
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, commandType);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(OpCodes.Callvirt, setter, null);
                il.Emit(OpCodes.Ret);
                action = (Action<IDbCommand, bool>)method.CreateDelegate(typeof(Action<IDbCommand, bool>));
            }
            // cache it             
            Link<Type, Action<IDbCommand, bool>>.TryAdd(ref _bindByNameCache, commandType, ref action);
            return action;
        }

        public AuditedDbCommand(DbCommand command, DbConnection connection, ISqlAuditor auditor)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            _command = command;
            _connection = connection;

            if (auditor != null) {
                _auditor = auditor;
            }
        }

        public override string CommandText
        {
            get { return _command.CommandText; }
            set { _command.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get { return _command.CommandTimeout; }
            set { _command.CommandTimeout = value; }
        }

        public override CommandType CommandType
        {
            get { return _command.CommandType; }
            set { _command.CommandType = value; }
        }

        protected override DbConnection DbConnection
        {
            get { return _connection; }
            set
            {
                // TODO: we need a way to grab the IDbProfiler which may not be the same as the MiniProfiler, it could be wrapped
                // allow for command reuse, it is clear the connection is going to need to be reset
                if (SqlAuditor.Current != null) {
                    _auditor = SqlAuditor.Current;
                }

                _connection = value;
                var auditedConnection = value as AuditedDbConnection;
                _command.Connection = auditedConnection == null ? value : auditedConnection.WrappedConnection;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _command.Parameters; }
        }

        protected override DbTransaction DbTransaction
        {
            get { return _transaction; }
            set
            {
                _transaction = value;
                var auditedTrans = value as AuditedDbTransaction;
                _command.Transaction = auditedTrans == null ? value : auditedTrans.WrappedTransaction;
            }
        }

        public override bool DesignTimeVisible
        {
            get { return _command.DesignTimeVisible; }
            set { _command.DesignTimeVisible = value; }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get { return _command.UpdatedRowSource; }
            set { _command.UpdatedRowSource = value; }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (_auditor == null) {
                return _command.ExecuteReader(behavior);
            }

            DbDataReader result = null;
            _auditor.ExecuteStart(this, ExecuteType.Reader);
            try {
                result = _command.ExecuteReader(behavior);
                result = new AuditedDbDataReader(result, _connection, _auditor);
            }
            catch (Exception e) {
                _auditor.OnError(this, ExecuteType.Reader, e);
                throw;
            }
            finally {
                _auditor.ExecuteFinish(this, ExecuteType.Reader, result);
            }

            return result;
        }

        public override int ExecuteNonQuery()
        {
            if (_auditor == null) {
                return _command.ExecuteNonQuery();
            }

            int result;
            _auditor.ExecuteStart(this, ExecuteType.NonQuery);
            try {
                result = _command.ExecuteNonQuery();
            }
            catch (Exception e) {
                _auditor.OnError(this, ExecuteType.NonQuery, e);
                throw;
            }
            finally {
                _auditor.ExecuteFinish(this, ExecuteType.NonQuery, null);
            }

            return result;
        }

        public override object ExecuteScalar()
        {
            if (_auditor == null) {
                return _command.ExecuteScalar();
            }

            object result;
            _auditor.ExecuteStart(this, ExecuteType.Scalar);
            try {
                result = _command.ExecuteScalar();
            }
            catch (Exception e) {
                _auditor.OnError(this, ExecuteType.Scalar, e);
                throw;
            }
            finally {
                _auditor.ExecuteFinish(this, ExecuteType.Scalar, null);
            }

            return result;
        }

        public override void Cancel()
        {
            _command.Cancel();
        }

        public override void Prepare()
        {
            _command.Prepare();
        }

        protected override DbParameter CreateDbParameter()
        {
            return _command.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _command != null) {
                _command.Dispose();
            }
            _command = null;
            base.Dispose(disposing);
        }

        public DbCommand InternalCommand { get { return _command; } }

        public AuditedDbCommand Clone()
        {
            // EF expects ICloneable 
            var tail = _command as ICloneable;
            if (tail == null) throw new NotSupportedException(
                string.Format("Underlying {0} is not cloneable", _command.GetType().Name));
            return new AuditedDbCommand((DbCommand)tail.Clone(), _connection, _auditor);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}