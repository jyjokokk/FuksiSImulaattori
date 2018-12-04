using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class FuksiSimulaattori : PhysicsGame
{
    private PlatformCharacter fuksi;
    private BoundingRectangle spawnAlue;
    private IntMeter pisteLaskuri;
    private Timer ajastin;
    private List<PhysicsObject> esteet = new List<PhysicsObject>();

    public override void Begin()
    {
        Gravity = new Vector(0.0, -800.0);
        AloitaPeli();
        AsetaOhjaimet();

        AddCollisionHandler<PlatformCharacter, PhysicsObject>(fuksi, "juoma", TormasiJuomaan);
        AddCollisionHandler<PlatformCharacter, PhysicsObject>(fuksi, "este", TormasiEsteeseen);

    }


    public void AloitaPeli()
    {
        LuoKentta();
        LuoPistelaskuri();
        for (int i = 0; i < 3; i++)
        {
            LuoJuoma(this, spawnAlue);
        }
        ajastin = new Timer();
        ajastin.Interval = 1.5;
        ajastin.Timeout += LuoEste;
        ajastin.Start();
    }


    /// <summary>
    /// Asettaa peliin ohjaimet pelihahmon liikuttamiseksi.
    /// </summary>
    public void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Up, ButtonState.Pressed, HahmoHyppaa, "Hahmo hyppaa", fuksi, 1500.0);
        Keyboard.Listen(Key.Left, ButtonState.Pressed, HahmoKavelee, "Hahmo liikkuu vasemmalle", fuksi, -800.0);
        Keyboard.Listen(Key.Right, ButtonState.Pressed, HahmoKavelee, "Hahmo liikkuu oikealle", fuksi, 800.0);
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// Luo kentan ja lisaa sinne pelihahmon.
    /// Maarittaa samalla spawnAlueen sijainnin ja koon.
    /// </summary>
    public void LuoKentta()
    {
        Level.CreateBorders();
        Camera.ZoomToLevel();
        spawnAlue = new BoundingRectangle(0.0, 0.0 - Level.Height / 4.0, Level.Width, Level.Height / 2.1);
        fuksi = new PlatformCharacter(40.0, 80.0);
        fuksi.X = Screen.Left + 80.0;
        fuksi.Y = Screen.Bottom + 10.0;
        fuksi.Shape = Shape.Rectangle;
        fuksi.Color = Color.Black;
        Add(fuksi);
    }

    /// <summary>
    /// Luo pistelaskurin oikeaan ylakulmaan.
    /// </summary>
    public void LuoPistelaskuri()
    {
        pisteLaskuri = new IntMeter(0);

        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Right - 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.White;
        pisteNaytto.IntFormatString = "Pisteita: {0:D2}";

        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }


    /// <summary>
    /// Pelihahmo hyppaa ylospain annetun impulssin voimakkuudella.
    /// </summary>
    /// <param name="hahmo">Pelihahmo.</param>
    /// <param name="impulssi">Impulssi.</param>
    public static void HahmoHyppaa(PlatformCharacter hahmo, double impulssi)
    {
        hahmo.Jump(impulssi);
    }


    /// <summary>
    /// Pelihahmo liikkuu annettuun suuntaan.
    /// </summary>
    /// <param name="hahmo">Pelihahmo.</param>
    /// <param name="vaakaNopeus">Nopeus ja suunta, mihin hahmo liikkuu.</param>
    public static void HahmoKavelee(PlatformCharacter hahmo, double vaakaNopeus)
    {
        hahmo.Walk(vaakaNopeus);
    }


    /// <summary>
    /// Aliohjelma, joka toteutuu pelihahmon tormatessa juomaan.
    /// </summary>
    /// <param name="hahmo">Pelaaja.</param>
    /// <param name="juoma">Juoma.</param>
    public void TormasiJuomaan(PlatformCharacter hahmo, PhysicsObject juoma)
    {
        juoma.IgnoresCollisionResponse = true;
        juoma.Destroy();
        LuoJuoma(this, spawnAlue);
        pisteLaskuri.Value += 1;
    }


    /// <summary>
    /// Lopettaa pelin pelaajan tormatessa esteeseen.
    /// </summary>
    /// <param name="hahmo">Pelaaja.</param>
    /// <param name="este">Este.</param>
    public void TormasiEsteeseen(PlatformCharacter hahmo, PhysicsObject este)
    {
        hahmo.Destroy();
        este.Stop();
        //ajastin.Stop();
        //for (int i = 0; i < esteet.Count; i++)
        //{
        //    esteet[i].Stop();
        //}
    }


    /// <summary>
    /// Luo peliin juoman satunnaiseen sijaintiin maaratylla spawnAlueelle.
    /// </summary>
    /// <returns>juoma.</returns>
    /// <param name="peli">Peli johon juoma luodaan.</param>
    /// <param name="rect">Maaratty alue, johon juoma luodaan.</param>
    public static void LuoJuoma(PhysicsGame peli, BoundingRectangle rect, string tunniste = "juoma")
    {
        PhysicsObject juoma = PhysicsObject.CreateStaticObject(15.0, 30.0);
        juoma.Shape = Shape.Rectangle;
        juoma.Color = Color.Red;
        juoma.Position = RandomGen.NextVector(rect);
        juoma.Tag = tunniste;
        peli.Add(juoma);
    }


    /// <summary>
    /// Luo peliin esteen.
    /// </summary>
    /// <returns>The este.</returns>
    public void LuoEste()
    {
        int korkeus = RandomGen.NextInt(100, 300);
        PhysicsObject este = PhysicsObject.CreateStaticObject(30.0, korkeus);
        este.Shape = Shape.Rectangle;
        este.Color = Color.Brown;
        este.X = Screen.Right;
        este.Y = Screen.Bottom + 50.0;
        este.Velocity = new Vector(-275.0, 0);
        this.Add(este);
    }
}
