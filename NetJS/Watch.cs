using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace NetJS {
    public class Watch {

        private class WatchFile {
            public DateTime LastModified;
            public Action OnChange;
        }

        private Dictionary<string, WatchFile> _files = new Dictionary<string, WatchFile>();

        public Watch() {
            var thread = new Thread(Loop);
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();
        }

        public void Loop() {
            while (true) {
                var keys = _files.Keys.ToArray();

                foreach (var key in keys) {
                    var modified = System.IO.File.GetLastWriteTime(key);
                    if (modified > _files[key].LastModified) {
                        _files[key].OnChange();
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void Add(string file, Action action) {
            _files[file] = new WatchFile() {
                LastModified = System.IO.File.GetLastWriteTime(file),
                OnChange = action
            };
        }
    }
}