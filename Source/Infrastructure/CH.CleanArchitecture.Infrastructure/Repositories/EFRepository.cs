using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Infrastructure.Models;

namespace CH.CleanArchitecture.Infrastructure
{
    ///<inheritdoc cref="IEFRepository{T, M, TId}"/>
    ///<summary>
    /// Repository implementation of <see cref="IAggregateRepository{T, M, TId}"/>.
    /// Operates on <see cref="AggregateRootBase{TId}"/> by having the underlying data entity of <see cref="DataEntityBase{TId}"/>
    /// </summary>
    internal abstract class EFRepository<T, M, TId> : IAggregateRepository<T, TId>
        where T : class, IAggregateRoot<TId>
        where M : class, IDataEntity<TId>
    {
        #region Private Fields

        private readonly IMapper _mapper;
        private readonly IEntityRepository<M, TId> _persistenceRepo;

        #endregion Private Fields

        #region Protected Fields

        public IUnitOfWork UnitOfWork => _persistenceRepo.UnitOfWork;

        #endregion Protected Fields

        #region Public Constructors

        public EFRepository(IMapper mapper, IEntityRepository<M, TId> persistenceRepo)
        {
            _mapper = mapper;
            _persistenceRepo = persistenceRepo;
        }

        #endregion Public Constructors

        #region Public Methods

        public virtual void Add(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));

            var dataEntity = _mapper.Map<M>(entity);
            _persistenceRepo.Add(dataEntity);
        }

        public virtual async ValueTask AddAsync(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));

            var dataEntity = _mapper.Map<M>(entity);
            await _persistenceRepo.AddAsync(dataEntity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            var dataEntities = _mapper.Map<IEnumerable<M>>(entities);
            _persistenceRepo.AddRange(dataEntities);
        }

        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            var dataEntities = _mapper.Map<IEnumerable<M>>(entities);
            return _persistenceRepo.AddRangeAsync(dataEntities);
        }

        public virtual void Delete(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));

            var dataEntity = _mapper.Map<M>(entity);
            _persistenceRepo.Delete(dataEntity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            Guard.Against.Null(entities, nameof(entities));

            var dataEntities = _mapper.Map<IEnumerable<M>>(entities);
            _persistenceRepo.DeleteRange(dataEntities);
        }

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            var expression = _mapper.Map<Expression<Func<M, bool>>>(predicate);
            return _persistenceRepo.Exists(expression);
        }

        public virtual Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            var expression = _mapper.Map<Expression<Func<M, bool>>>(predicate);
            return _persistenceRepo.ExistsAsync(expression);
        }

        public virtual IQueryable<T> GetAll()
        {
            return _mapper.ProjectTo<T>(_persistenceRepo.GetAll());
        }

        public virtual IQueryable<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            var expression = _mapper.Map<Expression<Func<M, bool>>>(predicate);
            return _mapper.ProjectTo<T>(_persistenceRepo.GetBy(expression));
        }

        public virtual T GetFirst(Expression<Func<T, bool>> predicate)
        {
            var expression = _mapper.Map<Expression<Func<M, bool>>>(predicate);
            return _mapper.Map<T>(_persistenceRepo.GetFirst(expression));
        }

        public virtual async Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate)
        {
            var expression = _mapper.Map<Expression<Func<M, bool>>>(predicate);
            return _mapper.Map<T>(await _persistenceRepo.GetFirstAsync(expression));
        }

        public virtual T GetSingle(Expression<Func<T, bool>> predicate)
        {
            var expression = _mapper.Map<Expression<Func<M, bool>>>(predicate);
            return _mapper.Map<T>(_persistenceRepo.GetSingle(expression));
        }

        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            var expression = _mapper.Map<Expression<Func<M, bool>>>(predicate);
            return _mapper.Map<T>(await _persistenceRepo.GetSingleAsync(expression));
        }

        public virtual void Update(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));

            var originalEntity = _persistenceRepo.Find(entity.Id);
            var updatedEntity = _mapper.Map(entity, originalEntity);
            _persistenceRepo.Update(updatedEntity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));

            var originalEntity = await _persistenceRepo.FindAsync(entity.Id);
            var updatedEntity = _mapper.Map(entity, originalEntity);
            _persistenceRepo.Update(updatedEntity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            Guard.Against.Null(entities, nameof(entities));

            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        public T Find(TId id)
        {
            return _mapper.Map<T>(_persistenceRepo.Find(id));
        }

        public async Task<T> FindAsync(TId id)
        {
            return _mapper.Map<T>(await _persistenceRepo.FindAsync(id));
        }

        #endregion Public Methods
    }
}