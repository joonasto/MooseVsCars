using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;
/// <summary>
/// T‰m‰ peli on hirvi vs autot
/// Alla olevassa koodissa m‰‰ritell‰‰n hirvelle
/// model ennalta luodusta spritest‰.
/// </summary>
public class hirviVsautot : PhysicsGame
{
    public override void Begin()
    {
        Image taustakuva = LoadImage("tausta1");
        Image hirvi = LoadImage("hirvimodel");
        Level.Size = new Vector(1000, 800);
        Level.Background.Image = taustakuva;
        Level.Background.FitToLevel();
        SetWindowSize(1000, 800);



        PhysicsObject pelaaja = new PhysicsObject(60, 60, Shape.FromImage(hirvi));
        pelaaja.Image = LoadImage("hirvimodel");
        Add(pelaaja);

        // T‰ss‰ kutsutaan "vihollista" eli autoa.
        for (int i = 0; i < 4; i++)
        {
            Riisikippo riisikippo = new Riisikippo(50, 50, new Color[] { Color.Red, Color.Yellow, Color.Green });
            riisikippo.Mass = 4;
            riisikippo.Position = RandomGen.NextVector(Level.BoundingRect);
            riisikippo.Image = LoadImage("riisikippo");
            Add(riisikippo);
            AddCollisionHandler<PhysicsObject, Riisikippo>(pelaaja, riisikippo, Tormays);
        }
        // T‰ss‰ m‰‰ritell‰‰n n‰pp‰imistˆn komennot.
        Keyboard.Listen(Key.Up, ButtonState.Down, Liikuta, "", pelaaja, new Vector(0, 2000));
        Keyboard.Listen(Key.Down, ButtonState.Down, Liikuta, "", pelaaja, new Vector(0, -2000));
        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "", pelaaja, new Vector(-2000, 0));
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "", pelaaja, new Vector(2000, 0));


        Level.CreateBorders();

        Camera.ZoomToLevel();

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    /// <summary>
    /// T‰m‰ aliohjelma m‰‰rittelee mit‰ tapahtuu,
    /// kun auto tˆrm‰‰ hirveen.
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <param name="kohde"></param>
    public void Tormays(PhysicsObject pelaaja, Riisikippo kohde)
    {
        kohde.TormaaHirveen();

    }

 


    public void Liikuta(PhysicsObject pelaaja, Vector suunta)
    {
        pelaaja.Push(suunta);
    }
}

