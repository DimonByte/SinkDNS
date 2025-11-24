using System;
using System.Collections.Generic;
using System.Text;

//MIT License

//Copyright (c) 2025 Dimon

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

namespace SinkDNS.Modules
{
    // This will manage updates for SinkDNS, and DNSCrypt, including checking for new versions and downloading them via the DownloadManager.
    internal class UpdateManager
    {
        //Static IsBlockListUpdateAvailable(BlocklistName As String) As Boolean
        //If a new version of the blocklist is available, return true, else return false.
        //Use DownloadManager to download the new blocklist if available.
        //If Local Blocklist doesn't match Remote Blocklist version, return true.
        //If Local blocklist doesn't exist, return true to force download.
        //InternetManager.LoadRemoteBlocklistVersion(BlocklistName) //Get the remote version of the blocklist.
        //LocalBlocklistVersion = FileManager.GetLocalBlocklistVersion(BlocklistName) //Get the local version of the blocklist.

        //Static IsSinkDNSUpdateAvailable() As Boolean
        //If a new version of SinkDNS is available, return true, else return false.

        //Static IsDNSCryptUpdateAvailable() As Boolean
        //If a new version of DNSCrypt is available, return true, else return false.
    }
}
