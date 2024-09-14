namespace Synchronization;

public interface IReaderWriterLock
{
    public void AcquireReaderLock();
    public void ReleaseReaderLock();
    public void AcquireWriterLock();
    public void ReleaseWriterLock();
}