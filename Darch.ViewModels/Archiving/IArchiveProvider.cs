// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Darch.ViewModels.Archiving
{
    public interface IArchiveProvider
    {
        Archive Open(string path);

        Archive Create(string path);
    }
}