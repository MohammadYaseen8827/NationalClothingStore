using Microsoft.EntityFrameworkCore.Storage;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Infrastructure.Data.Repositories;

namespace NationalClothingStore.Infrastructure.Data;

/// <summary>
/// Unit of Work implementation for managing transactions and repositories
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    public readonly NationalClothingStoreDbContext Context;
    
    object IUnitOfWork.Context => Context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(NationalClothingStoreDbContext context)
    {
        Context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public IRepository<T> Repository<T>() where T : class
    {
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (IRepository<T>)_repositories[typeof(T)];
        }

        var repository = new Repository<T>(Context);
        _repositories.Add(typeof(T), repository);
        return repository;
    }

    public IQueryable<T> Set<T>() where T : class
    {
        return Context.Set<T>();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await Context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        if (_transaction != null)
        {
            try
            {
                await _transaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        Context.Dispose();
    }
}
