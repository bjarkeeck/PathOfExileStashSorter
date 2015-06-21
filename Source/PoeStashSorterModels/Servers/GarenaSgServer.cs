namespace PoeStashSorterModels.Servers
{
    public class GarenaSgServer : GarenaCisServer
    {
        protected override string Domain
        {
            get { return "web.poe.garena.com"; }
        }

        public override string Name
        {
            get { return "GarenaSingapore"; }
        }
    }
}