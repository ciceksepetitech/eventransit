using System;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories;

public class ServiceLockMongoRepository : BaseMongoRepository<ServiceLock>, IServiceLockRepository
{
    public ServiceLockMongoRepository(IOptions<MongoDbSettings> mongoDbSettings,
        MongoDbConnectionStringBuilder connectionStringBuilder) : base(mongoDbSettings, connectionStringBuilder)
    {
    }

    public void Insert(ServiceLock data)
    {
        data.Id = Guid.NewGuid();
        data.LockStartDate = DateTime.Now;

        Collection.InsertOne(data);
    }

    public void Delete(string serviceName)
    {
        Collection.FindOneAndDelete(x => x.ServiceName == serviceName);
    }

    public ServiceLock GetByServiceName(string serviceName)
    {
        return Collection.Find(x => x.ServiceName == serviceName).FirstOrDefault();
    }
}
