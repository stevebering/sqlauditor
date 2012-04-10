using System;
using System.Data.Common;

namespace Meracord.Data.SqlAuditor
{
    public interface ISqlAuditor
    {
        /// <summary> 
        /// Called when a command starts executing 
        /// </summary> 
        /// <param name="command"></param> 
        /// <param name="executeType"></param>  
        void ExecuteStart(DbCommand command, ExecuteType executeType);

        /// <summary> 
        /// Called when an error happens during execution of a command  
        /// </summary> 
        /// <param name="command"></param> 
        /// <param name="executeType"></param> 
        /// <param name="exception"></param> 
        void OnError(DbCommand command, ExecuteType executeType, Exception exception);

        /// <summary> 
        /// Called when a reader finishes executing 
        /// </summary> 
        /// <param name="command"></param> 
        /// <param name="executeType"></param> 
        /// <param name="reader"></param>  
        void ExecuteFinish(DbCommand command, ExecuteType executeType, DbDataReader reader);

        /// <summary> 
        /// Called when a reader is done iterating through the data  
        /// </summary> 
        /// <param name="reader"></param>  
        void ReaderFinish(DbDataReader reader);
    }
}