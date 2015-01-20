using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodePrinter.Logging
{
    class NLogLogger
    {
        #region Fields
        private readonly NLog.Logger _innerLogger;
        #endregion

        #region Constructors
        public NLogLogger(Type type)
        {
            _innerLogger = NLog.LogManager.GetLogger(type.Name);
        }
        #endregion

        #region ILog Members
        public void Error(Exception exception)
        {
            _innerLogger.ErrorException(exception.Message, exception);
        }
        public void Info(string format, params object[] args)
        {
            _innerLogger.Info(format, args);
        }
        public void Warn(string format, params object[] args)
        {
            _innerLogger.Warn(format, args);
        }
        #endregion
    }
}
