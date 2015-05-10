// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Darch.ViewModels.ViewModels
{
    public interface IMainViewModel
    {
        ViewModelStatus Status { get; set; }

        ulong TotalWork { get; set; }

        ulong WorkDone { get; set; }

        string Name { get; set; }
        
        int Size { get; set; }

        int UnpackSize { get; set; }

        void Create(string path);
        
        void Open(string path);

        void Add(string path);

        void Remove(string name);

        void Extract(string name);
    }
}
