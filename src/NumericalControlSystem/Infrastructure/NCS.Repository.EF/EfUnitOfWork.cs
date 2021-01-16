#region 关于UnitofWork的使用
/*关于UnitofWork的使用
     * 假设OrderSerer使用实现了UnitofWork的Repository
     * 
     * 
*  public interface IOrderRepository : IRepository<Order, int>
{
}

    public interface IUnitOfWork
    {
        void RegisterAmended(IAggregateRoot entity,
                             IUnitOfWorkRepository unitofWorkRepository);
        void RegisterNew(IAggregateRoot entity,
                         IUnitOfWorkRepository unitofWorkRepository);
        void RegisterRemoved(IAggregateRoot entity,
                             IUnitOfWorkRepository unitofWorkRepository);
        void Commit();
    }
 
 * 
 
 * namespace NCS.Repository.EF
{
    public abstract class RepositoryBase<T, TEntityKey> 
        : IUnitOfWorkRepository 
        where T : IAggregateRoot
    {
        private IUnitOfWork _unitOfWork;

        public RepositoryBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
            set { _unitOfWork = value; }
        }

        #region //用UnitOfWorK的_uow对象对实体的持久化操作（增删改，不包括查）进行注册登记标示
        public void Add(T entity)
        {
            _unitOfWork.RegisterNew(entity, this);
        }

        public void Remove(T entity)
        {
            _unitOfWork.RegisterRemoved(entity, this);
        }

        public void Save(T entity)
        {
            _unitOfWork.RegisterAmended(entity, this);
        }
 *      #endregion
 * 
 
 * 
 * 
 * 
        namespace Agathas.Storefront.Services.Implementations
        {
            public class OrderService : IOrderService
            {
                private readonly ICustomerRepository _customerRepository;
                private readonly IOrderRepository _orderRepository;
                private readonly IBasketRepository _basketRepository;
                private readonly IUnitOfWork _uow;

                public OrderService(IOrderRepository orderRepository,
                                    IBasketRepository basketRepository,
                                    ICustomerRepository customerRepository,
                                    IUnitOfWork uow)
                {
                    _customerRepository = customerRepository;
                    _orderRepository = orderRepository;
                    _basketRepository = basketRepository;
                    _uow = uow;
                }
     * 
     * 。。。。
            public CreateOrderResponse CreateOrder(CreateOrderRequest request)
            {
                CreateOrderResponse response = new CreateOrderResponse();
                Customer customer = _customerRepository
                                    .FindBy(request.CustomerIdentityToken);
                Basket basket = _basketRepository.FindBy(request.BasketId);

                DeliveryAddress deliveryAddress = customer.DeliveryAddressBook
                                .Where(d => d.Id == request.DeliveryId).FirstOrDefault();

                Order order = basket.ConvertToOrder();

                order.Customer = customer;
                order.DeliveryAddress = deliveryAddress;

                _orderRepository.Save(order);
                _basketRepository.Remove(basket);
                _uow.Commit();

                response.Order = order.ConvertToOrderView();

                return response;
            }
 */
        #endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.UnitOfWork;
using NCS.Infrastructure.Domain;

namespace NCS.Repository.EF
{
    public class EfUnitOfWork : IUnitOfWork 
    {                 
        public void Commit()
        {
            //TODO:添加事务并发处理代码
            //DataContextFactory.GetDataContext().SaveChanges();  


            //            try
            //            {
            //                using(var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            //                {
            //                    this.Context.SaveChanges();
            //                    scope.Complete();
            //                }
            //           }
            //           catch (DbUpdateConcurrencyException concurrencyException)
            //           {
            //         
            //                  //EF框架处理并发的方式：
            //                  //   方式1：
            //                  //   让客户端决定状态----让项的原始值设置为从数据库中获取的值：
            //                  //   concurrencyException.Entries.Single().OriginalValues.SetValues(concurrencyException.Entries.Single().GetDatabaseValues());
            //
            //                  //   方式2：
            //                  //   让客户端决定状态----用存储中的值刷新新实体值：
            //                  //   concurrencyException.Entries.Single().Reload（）;
            //
            //                  //   方式3：
            //                  //   自定义一种方案，选择合适的选项。
            //                  concurrencyException.Entries.Single().OriginalValues.SetValues(concurrencyException.Entries.Single().GetDatabaseValues());
            //           }
        }

        public void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository)
        {
            unitofWorkRepository.PersistCreationOf(entity); 
        }

        public void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository)
        {
            unitofWorkRepository.PersistDeletionOf(entity); 
        }

        public void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository)
        {
            unitofWorkRepository.PersistUpdateOf(entity);
        }
    }
}
