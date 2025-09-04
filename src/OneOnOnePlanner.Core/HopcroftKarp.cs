namespace OneOnOnePlanner.Core;

public class HopcroftKarp
{
    public readonly int U, V;
    public readonly List<int>[] Adj;
    public int[] PairU; // matched v for u or -1
    public int[] PairV; // matched u for v or -1
    int[] Dist;

    public HopcroftKarp(int uCount, int vCount, List<int>[] adj)
    {
        U = uCount; V = vCount; Adj = adj;
        PairU = Enumerable.Repeat(-1, U).ToArray();
        PairV = Enumerable.Repeat(-1, V).ToArray();
        Dist = new int[U];
    }

    public int MaxMatching()
    {
        int matching = 0;
        while (Bfs())
        {
            for (int u = 0; u < U; u++)
                if (PairU[u] == -1 && Dfs(u))
                    matching++;
        }
        return matching;
    }

    bool Bfs()
    {
        var q = new Queue<int>();
        for (int u = 0; u < U; u++)
        {
            if (PairU[u] == -1) { Dist[u] = 0; q.Enqueue(u); }
            else Dist[u] = int.MaxValue;
        }

        bool reachableFree = false;
        while (q.Count > 0)
        {
            int u = q.Dequeue();
            foreach (var v in Adj[u])
            {
                int u2 = PairV[v];
                if (u2 != -1 && Dist[u2] == int.MaxValue)
                {
                    Dist[u2] = Dist[u] + 1;
                    q.Enqueue(u2);
                }
                if (u2 == -1) reachableFree = true;
            }
        }
        return reachableFree;
    }

    bool Dfs(int u)
    {
        foreach (var v in Adj[u])
        {
            int u2 = PairV[v];
            if (u2 == -1 || (Dist[u2] == Dist[u] + 1 && Dfs(u2)))
            {
                PairU[u] = v;
                PairV[v] = u;
                return true;
            }
        }
        Dist[u] = int.MaxValue;
        return false;
    }
}