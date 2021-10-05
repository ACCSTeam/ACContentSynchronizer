using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Avalonia.Threading;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Extensions {
  public static class ReactiveExtensions {
    public static void SubscribeValue<TSender, TRet>(this TSender sender, Expression<Func<TSender, TRet>> property1,
                                                     Action<TRet> action) {
      sender.WhenAnyValue(property1).Throttle(TimeSpan.FromSeconds(.25), AvaloniaScheduler.Instance).Subscribe(action);
    }
  }
}
