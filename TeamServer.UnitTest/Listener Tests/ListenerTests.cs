using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamServer.Models;
using TeamServer.Services;

namespace TeamServer.UnitTest.Listener_Tests
{
    public class ListenerTests
    {
        private readonly IListenerService _listeners;

        public ListenerTests(IListenerService listeners)
        {
            _listeners = listeners;
        }

        [Fact]
        public void TestCreateGetListener()
        {
            var originalListener = new HttpListener("TestListener", 4444);
            _listeners.AddListener(originalListener);

            var newListener = (HttpListener)_listeners.GetListener(originalListener.Name);

            Assert.NotNull(newListener);
            Assert.Equal(originalListener.Name, newListener.Name);
            Assert.Equal(originalListener.BindPort, newListener.BindPort);
        }
    }
}
