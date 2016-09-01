using System;
using System.Collections.Generic;
namespace Ik.Framework.SerialNumber
{
    public interface ISerialNumberService
    {
        string GetSerialNumber();

        IList<string> GetSerialNumber(int count);
    }
}
