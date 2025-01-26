public struct DefenseData
{
    public float PhysicalResistance { get; private set; }
    public float CritResistance { get; private set; }

    public DefenseData(float physicalResistance, float critResistance)
    {
        PhysicalResistance = physicalResistance;
        CritResistance = critResistance;
    }
}
