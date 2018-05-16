namespace CloudSim.Sharp
{
    public class UtilizationModelNull : UtilizationModel
    {
        public double GetUtilization(double time)
        {
            return 0;
        }
    }
}
