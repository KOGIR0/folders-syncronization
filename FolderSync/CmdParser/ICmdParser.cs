using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FolderSync.CommandLine
{
    interface ICmdParser
    {
        public Task<int> invoke(string[] args);
    }
}
