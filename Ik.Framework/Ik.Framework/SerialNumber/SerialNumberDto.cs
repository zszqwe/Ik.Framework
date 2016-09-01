using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.SerialNumber
{
    public class SerialNumberDto
    {
        [DataAccess.Alias("seq_key")]
        public string Key { get; set; }
        public string Name { get; set; }
        public long NextValue { get; set; }
        public string PrefixValue { get; set; }

        private int formatLength;
        public int FormatLength 
        {
            get
            {
                return formatLength;
            }
            set
            {
                formatLength = value;
                if (formatLength > 0)
                {
                    StringBuilder format = new StringBuilder();
                    StringBuilder formatMaxvalue = new StringBuilder();
                    format.Append("{0:");
                    formatMaxvalue.Append("1");
                    for (int i = 0; i < formatLength; i++)
                    {
                        format.Append("0");
                        formatMaxvalue.Append("0");
                    }
                    format.Append("}");
                    lengthTemplete = format.ToString();
                    lengthMaxValue = Convert.ToInt64(formatMaxvalue.ToString()) - 1;
                }
            }
        }
        public string DateFormat { get; set; }

        public int CheckThreshold { get; set; }

        private string lengthTemplete;
        public string LengthTemplete 
        {
            get
            {
                return lengthTemplete;
            }
        }

        private long lengthMaxValue;
        public long LengthMaxValue 
        {
            get
            {
                return lengthMaxValue;
            }
        }

    }
}
