using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CodeFirstProfiledEF.Models;

namespace CodeFirstProfiledEF.Framework
{
    public class AuditedDbConnection
        : DbConnection, ICloneable
    {
        private DbConnection _connection;
        private IDbAuditor _auditor;

        public IDbAuditor Auditor
        {
            get { return _auditor; }
        }

        public AuditedDbConnection(DbConnection connection, IDbAuditor auditor)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            _connection = connection;
            _connection.StateChange += StateChangeHandler;

            if (auditor != null) {
                _auditor = auditor;
            }
        }

        public DbConnection InnerConnection
        {
            get { return _connection; }
        }

        public DbConnection WrappedConnection
        {
            get { return _connection; }
        }

        protected override bool CanRaiseEvents
        {
            get { return true; }
        }

        public override string ConnectionString
        {
            get { return _connection.ConnectionString; }
            set { _connection.ConnectionString = value; }
        }

        public override int ConnectionTimeout
        {
            get { return _connection.ConnectionTimeout; }
        }

        public override string Database
        {
            get { return _connection.Database; }
        }

        public override string DataSource
        {
            get { return _connection.DataSource; }
        }

        public override string ServerVersion
        {
            get { return _connection.ServerVersion; }
        }

        public override ConnectionState State
        {
            get { return _connection.State; }
        }

        public override void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _connection.Close();
        }

        public override void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            _connection.EnlistTransaction(transaction);
        }

        public override DataTable GetSchema()
        {
            return _connection.GetSchema();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return _connection.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return _connection.GetSchema(collectionName, restrictionValues);
        }

        public override void Open()
        {
            _connection.Open();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new AuditedDbTransaction(_connection.BeginTransaction(isolationLevel), this);
        }

        protected override DbCommand CreateDbCommand()
        {
            return new AuditedDbCommand(_connection.CreateCommand(), this, _auditor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _connection != null) {
                _connection.StateChange -= StateChangeHandler;
                _connection.Dispose();
            }

            _connection = null;
            _auditor = null;
            base.Dispose(disposing);
        }

        void StateChangeHandler(object sender, StateChangeEventArgs e)
        {
            OnStateChange(e);
        }

        public AuditedDbConnection Clone()
        {
            ICloneable tail = _connection as ICloneable;
            if (tail == null) {
                throw new NotSupportedException(
                    string.Format("Underlying {0} is not cloneable", _connection.GetType().Name));
            }
            return new AuditedDbConnection((DbConnection)tail.Clone(), _auditor);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                var factory = base.DbProviderFactory;
                return factory;
            }
        }
    }
}