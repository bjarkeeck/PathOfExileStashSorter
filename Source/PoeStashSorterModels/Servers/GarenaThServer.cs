namespace PoeStashSorterModels.Servers
{
    public class GarenaThServer : GarenaCisServer
    {
        protected override string Domain
        {
            get { return "web.poe.garena.in.th"; }
        }

        public override string Name
        {
            get { return "GarenaThailand"; }
        }
    }
}