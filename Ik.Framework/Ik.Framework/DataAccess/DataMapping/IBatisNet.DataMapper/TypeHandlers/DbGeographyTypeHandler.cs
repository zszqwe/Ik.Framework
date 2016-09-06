
using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNet.DataMapper.TypeHandlers
{
    public class DbGeographyTypeHandler : ITypeHandler
    {

        public bool IsSimpleType
        {
            get { return true; }
        }

        public object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int index = dataReader.GetOrdinal(mapping.ColumnName);

            if (dataReader.IsDBNull(index) || dataReader.GetBytes(index, 0, null, 0, 0) == 0)
            {
                return DBNull.Value;
            }
            else
            {
                return GetValueByIndex(index, dataReader);
            }
        }

        public object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex) || dataReader.GetBytes(mapping.ColumnIndex, 0, null, 0, 0) == 0)
            {
                return DBNull.Value;
            }
            else
            {
                return GetValueByIndex(mapping.ColumnIndex, dataReader);
            }
        }


        private DbGeography GetValueByIndex(int columnIndex, IDataReader dataReader)
        {
            byte[] dbBytes = null;
            int bufferLength = (int)dataReader.GetBytes(columnIndex, 0, null, 0, 0);

            // initialize it
            byte[] byteArray = new byte[bufferLength];

            // fill it
            dataReader.GetBytes(columnIndex, 0, byteArray, 0, bufferLength);
            var sqlBytes = new SqlBytes(byteArray);
            //dbBytes = SqlGeography.Deserialize(sqlBytes).AsBinaryZM().Value;
            return DbGeography.FromBinary(dbBytes);
        }

        public object GetDataBaseValue(object outputValue, Type parameterType)
        {
            throw new NotImplementedException("DbGeographyTypeHandler");
        }

        public void SetParameter(System.Data.IDataParameter dataParameter, object parameterValue, string dbType)
        {
            if (parameterValue == null)
            {
                dataParameter.Value = DBNull.Value;
                dataParameter.DbType = DbType.Binary;
            }
            else
            {
                DbGeography geo = (DbGeography)parameterValue;
                //var sqlDbGeography = SqlGeography.Parse(geo.AsText());
                //dataParameter.Value = sqlDbGeography.Serialize().Value;
            }
        }

        public object ValueOf(Type type, string s)
        {
            throw new NotImplementedException("DbGeographyTypeHandler");
        }

        public bool Equals(object obj, string str)
        {
            return true;
        }

        public object NullValue
        {
            get { return null; }
        }
    }
}

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
