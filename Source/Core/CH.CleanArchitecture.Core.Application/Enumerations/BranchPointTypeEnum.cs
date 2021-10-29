namespace CH.CleanArchitecture.Core.Application
{
    public enum BranchPointTypeEnum
    {
        /// <summary>
        /// An out-of-order event is one that's received late, sufficiently late that you've already processed events that should have been processed after the out-of-order event was received.
        /// For out-of-order events we insert and process the retroactive event at the branch point.
        /// Effectively an addition of the event after the branch point
        /// </summary>
        OutOfOrder,

        /// <summary>
        /// A rejected event is an event that you now realize was false and should never have been processed.
        /// For rejected events we reverse the event and mark it as rejected.
        /// Effectively a delete to the event log.
        /// </summary>
        Rejected,

        /// <summary>
        /// A incorrect event is an event where you received incorrect information about the event.
        /// For incorrect events we reverse the original event and mark it as rejected. We insert the retroactive event and process it.
        /// You can think of this as a rejected event and an out of order event being processed as one.
        /// Effectively a replacement of the event.
        /// </summary>
        Incorrect
    }
}