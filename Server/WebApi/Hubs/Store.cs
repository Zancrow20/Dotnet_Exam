using System.Collections.Concurrent;
using Contracts;

namespace WebApi.Hubs;

public class Store
{
    public readonly ConcurrentDictionary<string, HashSet<string>> GameConnections = new();
    public readonly ConcurrentDictionary<string, UserGroups> UserGroupsConnections = new();
    public readonly ConcurrentDictionary<string, UserMove> UsersMove = new();
}

