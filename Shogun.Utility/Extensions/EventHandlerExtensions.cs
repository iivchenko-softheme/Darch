// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;

namespace Shogun.Utility.Extensions
{
    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for querying objects that implement <see cref="EventHandler{TEventArgs}"/>.
    /// </summary>
    public static class EventHandlerExtensions
    {
        /// <summary>
        /// Safe raise of an event.
        /// </summary>
        /// <typeparam name="TEventArgs">Type of the event arguments.</typeparam>
        /// <param name="eventHandler">Event to be raised.</param>
        /// <param name="sender">The raiser of the event.</param>
        /// <param name="args">Event args.</param>
        public static void OnEvent<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs args) where TEventArgs : EventArgs
        {
            var handler = eventHandler;

            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }
}
