using System;
using System.Data;
using System.Data.Common;

namespace CodeFirstProfiledEF.Framework
{
    public class AuditedDbTransaction
        : DbTransaction
    {
        private AuditedDbConnection _connection;
        private DbTransaction _transaction;

        public AuditedDbTransaction(DbTransaction transaction, AuditedDbConnection connection)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (connection == null) throw new ArgumentNullException("connection");
            _transaction = transaction;
            _connection = connection;
        }

        protected override DbConnection DbConnection
        {
            get { return _connection; }
        }

        internal DbTransaction WrappedTransaction
        {
            get { return _transaction; }
        }

        public override IsolationLevel IsolationLevel
        {
            get { return _transaction.IsolationLevel; }
        }

        public override void Rollback()
        {
            _transaction.Rollback();
        }

        public override void Commit()
        {
            _transaction.Commit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _transaction != null)
            {
                _transaction.Dispose();
            }

            _transaction = null;
            _connection = null;
            base.Dispose(disposing);
        }
    }
}