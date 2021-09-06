using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Anna
{
    public static class StreamExtensions
    {
        public static IObservable<byte[]> ReadBytes(this Stream stream, int count)
        {
            var buffer = new byte[count];

            var ret = Observable.FromAsync(() => stream.ReadAsync(buffer, 0, count));

            return ret.Select(u => buffer);


            /* 2#Ð´·¨
            var ret1 = Task.Factory.FromAsync(stream.BeginRead, ar =>
                                                   {
                                                       stream.EndRead(ar);
                                                       return buffer;
                                                   }, buffer, 0, count, null);

            */

            /*
            return Observable.FromAsyncPattern((cb, state) => stream.BeginRead(buffer, 0, count, cb, state), ar =>
            {
                stream.EndRead(ar);
                return buffer;
            })();
            */
        }
    }
}