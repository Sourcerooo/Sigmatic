namespace Sigmatic.App.Interface.Data;

using Sigmatic.Core.Model;

public interface ITransactionLoader
{
    List<RawTransaction> LoadAll(string folderPath);
}