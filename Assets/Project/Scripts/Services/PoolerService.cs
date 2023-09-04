using UnityEngine;

public class PoolerService
{
    public Pooler<Chunk> poolerChunk;
    public Pooler<BonusCircle> poolerGate;
    
    // -------------------------------------------------------------------------------

    public void InitGatePooler(BonusCircle gate, int poolSize)
    {
        poolerGate = new Pooler<BonusCircle>(gate, poolSize);
    }

    public BonusCircle GetGate(Vector3 pos, Quaternion qt)
    {
        return poolerGate.Get(pos, qt);
    }

    public void FreeGate(BonusCircle gate)
    {
        poolerGate.Free(gate);
    }

    // -------------------------------------------------------------------------------

    public void InitChunkPooler(Chunk chunk, int poolSize, Transform parent)
    {
        poolerChunk = new Pooler<Chunk>(chunk, poolSize, parent);
    }

    public Chunk GetChunk()
    {
        return poolerChunk.Get();
    }

    public void FreeChunk(Chunk chunk)
    {
        poolerChunk.Free(chunk);
    }
}
