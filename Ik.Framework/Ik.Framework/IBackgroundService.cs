using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework
{
    public interface IBackgroundService
    {
        event EventHandler<ErrorEventArgs> ServiceException;
        bool IsRunning { get; }
        void Start();
        void Stop();
    }
}
