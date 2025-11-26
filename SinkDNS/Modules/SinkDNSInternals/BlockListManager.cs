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

namespace SinkDNS.Modules.SinkDNSInternals
{
    //This will manage the block lists for SinkDNS, including downloading, updating, and parsing them.
    //There will be a list of the block lists that are the most popular on this repo that SinkDNS references.
    //BlockListCompression as well, that will remove any # comments and blank lines from the block lists to reduce their size.
    internal class BlockListManager
    {
        //CheckBlockListForUpdates
        //ForEachSelectedBlockList in Settings
        //IsBlockListUpdateAvailable() from UpdateManager
        //If IsBlockListUpdateAvailable() == true
        //DownloadManager.DownloadBlockList(BlockListItem)
        //After For each
        //ParseBlockLists() //This will combine all the block lists into one file for DNSCrypt to use.
        //ServiceManager.RestartDNSCryptService() //Restart the DNSCrypt service to apply the new block lists.

        public static void MergeBlockLists()
        {
            //This will merge all the block lists into one file for DNSCrypt to use.
            //
        }
        public static void AddToBlockList(string domain)
        {
            //This will add a domain to the custom block list.
        }
        public static void RemoveFromBlockList(string domain)
        {
            //This will remove a domain from the custom block list.
        }
        public static bool IsDomainBlocked(string domain)
        {
            //This will check if a domain is in the block list.
            return false;
        }
        public static void ClearBlockList()
        {
            //This will clear the custom block list.
        }
        public static void WhitelistDomain(string domain)
        {
            //This will add a domain to the whitelist, so it won't be blocked.
        }
        public static void RemoveFromWhitelist(string domain)
        {
            //This will remove a domain from the whitelist.
        }
        public static bool IsDomainWhitelisted(string domain)
        {
            //This will check if a domain is in the whitelist.
            return false;
        }
        public static void ClearWhitelist()
        {
            //This will clear the whitelist.
        }
    }
}
