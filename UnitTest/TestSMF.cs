using System.Text;
using libMidi.SMF;

namespace UnitTest
{
    public static class Properties
    {
        public const string SMF_FILE_PATH1 = @"H:\ProgramData\PreSonus\Studio One\Songs\★Rock\You're Not Here\midi\Akira Yamaoka-You're Not Here-02-04-2009.mid";
        public const string SMF_FILE_PATH2 = @"F:\その他\MIDIデータ\Music\Yamaha Midi Data\六本木純情派\XG5795K1.MID";
    }

    [TestClass]
    public sealed class TestSMF
    {
        public TestContext TestContext { get; set; } = null!;

        [TestInitialize]
        public void TestInit()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void TestLoadSMF()
        {
            ConverterSetting.Root = @"F:\projects\QB\libQB\UnitTest";

            var midi1 = SMFLoader.Load(Properties.SMF_FILE_PATH1);
            midi1.Organize();

            var midi1c = new MidiData(midi1.Division);

            SMFConverter.Convert(libMidi.SMF.enums.ConvertType.MultiTimber, midi1, midi1c);

            foreach (var track in midi1c.Tracks)
            {
                TestContext.WriteLine($"{track}");
                foreach (var midiEvent in track.Events)
                {
                    TestContext.WriteLine($"{midiEvent}");
                }
            }
        }
    }
}
