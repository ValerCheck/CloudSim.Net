using System;
using System.Collections.Generic;

namespace CloudSim.Sharp.Core
{
    public class SimulationNull : Simulation
    {
        public override bool IsTerminationTimeSet => false;
        public override void Abort() {}
        public override void AddEntity(CloudSimEntity entity) {}
        public override SimEvent Cancel(SimEntity src, Predicate<SimEvent> predicate) => SimEvent.NULL;
        public override bool CancelAll(SimEntity src, Predicate<SimEvent> predicate) => false;
        public override double Clock => 0.0;
        public override string ClockStr => string.Empty;
        public override double ClockInMinutes => 0.0;
        public override double ClockInHours => 0.0;
        public override SimEvent FindFirstDeferred(SimEntity dest, Predicate<SimEvent> predicate) => SimEvent.NULL;

        public override CloudInformationService CloudInfoService => null;

        public override List<SimEntity> EntityList => new List<SimEntity>();
        public override double MinTimeBetweenEvents => 0;
        public override int NumEntities => 0;
        public override bool RemoveOnEventProcessingListener(EventHandler<SimEvent> listener) => false;
        public override bool RemoveOnSimulationPauseListener(EventHandler<EventArgs> listener) => false;
        public override bool IsPaused => false;
        public override void PauseEntity(SimEntity src, double delay) {/**/}
        public override bool Pause() => false;
        public override bool Pause(double time) => false;
        public override bool Resume() => false;
        public override bool IsRunning => false;
        public override SimEvent Select(SimEntity dest, Predicate<SimEvent> predicate) => SimEvent.NULL;
        public override void Send(SimEvent evt) {/**/}
        public override void Send(SimEntity src, SimEntity dest, double delay, int tag, object data) {}
        public override void SendFirst(SimEvent evt) {/**/}
        public override void SendFirst(SimEntity src, SimEntity dest, double delay, int tag, Object data) {/**/}
        public override void SendNow(SimEntity src, SimEntity dest, int tag, object data) {}
        public override double RunFor(double interval) => 0;
        public override bool RemoveOnClockTickListener(EventHandler<EventArgs> listener) => false;
        public override double Start() => 0;
        public override void StartSync() { /**/ }
        public override bool IsTimeToTerminateSimulationUnderRequest() => false;
        public override bool Terminate() => false;
        public override bool TerminateAt(double time) => false;
        public override void Wait(CloudSimEntity src, Predicate<SimEvent> predicate) {}

#warning to be done
        #region Network
        //public NetworkTopology GetNetworkTopology() { return NetworkTopology.NULL; }
        //public void setNetworkTopology(NetworkTopology networkTopology) {/**/}
        #endregion

        public override long GetNumberOfFutureEvents(Predicate<SimEvent> predicate) => 0;
        public override double LastCloudletProcessingUpdate { get; set; } = 0;
    }
}
