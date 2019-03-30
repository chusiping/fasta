using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fasta2011
{
    public class LogMa
    {
        public string LogPreName { get; set; }
        public void WriteLog(string log, string LogCustName = "log.txt")
        {
            var LogWriteLock = new ReaderWriterLockSlim();
            string LogFileName = LogPreName + LogCustName;
            try
            {
                LogWriteLock.EnterWriteLock();
                System.IO.File.AppendAllText(LogFileName, log);
                System.IO.File.AppendAllText(LogFileName, "\r\n");
                Console.WriteLine(log);
            }
            catch (Exception)
            {
                //FailedCount++;
            }
            finally
            {
                LogWriteLock.ExitWriteLock();
            }
        }
    }
}
