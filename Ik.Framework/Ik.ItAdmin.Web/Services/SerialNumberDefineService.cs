using Ik.Framework.Common.Paging;
using Ik.Framework.DataAccess.EF;
using Ik.Framework.DependencyManagement;
using Ik.ItAdmin.Web.DataAccess.EF;
using Ik.ItAdmin.Web.Models;
using Ik.WebFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ik.Framework.Common.Extension;
using Ik.ItAdmin.Web.DataAccess;

namespace Ik.ItAdmin.Web.Services
{
    [AutoBizServiceAttribute]
    public class SerialNumberDefineService
    {
        private IRepository<SerialDefineInfoEntity> _efSerialDefineInfoRepository = null;

        public SerialNumberDefineService(IRepository<SerialDefineInfoEntity> efSerialDefineInfoRepository)
        {
            this._efSerialDefineInfoRepository = efSerialDefineInfoRepository;
        }

        public IPagedList<SerialDefineInfoModel> GetSerialDefineInfoListByPager(GridQuery filter)
        {
            var list = this._efSerialDefineInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            int count = this._efSerialDefineInfoRepository.TableNoTracking.Count();

            return list.Select(ser =>
            {
                return new SerialDefineInfoModel
                {
                    Id = ser.Id,
                    Key = ser.Key,
                    CreateTime = ser.CreateTime,
                    Desc = ser.Desc,
                    Name = ser.Name,
                    PrefixValue = ser.PrefixValue,
                    NextValue = ser.NextValue,
                    DateFormat = ser.DateFormat,
                    ApplyCapacity = ser.ApplyCapacity,
                    CheckThreshold = ser.CheckThreshold,
                    FormatLength = ser.FormatLength
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);

        }

        public void AddOrUpdateAllSerialDefineInfo(SerialDefineInfoModel model)
        {
            if (model.Id.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var serInfo = this._efSerialDefineInfoRepository.GetById(model.Id.Value);
                    serInfo.Name = model.Name;
                    serInfo.Desc = model.Desc;
                    serInfo.ApplyCapacity = model.ApplyCapacity;
                    serInfo.CheckThreshold = model.CheckThreshold;
                    serInfo.DateFormat = model.DateFormat;
                    serInfo.FormatLength = model.FormatLength;
                    serInfo.PrefixValue = model.PrefixValue;
                    this._efSerialDefineInfoRepository.Update(serInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efSerialDefineInfoRepository.Insert(new SerialDefineInfoEntity
                {
                    Id = Guid.NewGuid(),
                    Key = model.Key,
                    CreateTime = DateTime.Now,
                    Desc = model.Desc,
                    Name = model.Name,
                    PrefixValue = model.PrefixValue,
                    DateFormat = model.DateFormat,
                    ApplyCapacity = model.ApplyCapacity,
                    CheckThreshold = model.CheckThreshold,
                    FormatLength = model.FormatLength
                });
            }
        }

        public SerialDefineInfoModel GetSerialDefineInfo(Guid id)
        {
            var ser = this._efSerialDefineInfoRepository.GetById(id);
            return new SerialDefineInfoModel
            {
                Id = ser.Id,
                Key = ser.Key,
                CreateTime = ser.CreateTime,
                Desc = ser.Desc,
                Name = ser.Name,
                PrefixValue = ser.PrefixValue,
                NextValue = ser.NextValue,
                DateFormat = ser.DateFormat,
                ApplyCapacity = ser.ApplyCapacity,
                CheckThreshold = ser.CheckThreshold,
                FormatLength = ser.FormatLength
            };
        }

        public bool DeleteAllSerialDefineInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var serInfo = this._efSerialDefineInfoRepository.GetById(item);
                    this._efSerialDefineInfoRepository.Delete(serInfo);
                }
                scope.SaveChanges();
            }
            return true;
        }
    }
}