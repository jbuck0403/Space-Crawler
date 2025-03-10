public interface IStrategy
{
    // ptional helper methods for checking state conditions
    bool CanExit() => true;
    bool IsComplete() => false;
    void OnStrategyComplete(); // called when strategy reads IsComplete() == true
}
