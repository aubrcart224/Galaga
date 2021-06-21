using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Galaga
{

    public partial class Form1 : Form
    {
        //global varibles 

        int playerSpeed = 10;
        int score = 0;
        int timer = 0;
        int round = 0;
        int lives = 3;
        
        
        string gameState = "waiting";
        int wave = 1;
        int easyBulletSpeed = 8;
        int mediumBulletSpeed = 10;
        int bulletSpeed = -15;

        Image easyImage = Properties.Resources.eastEnemey;
        Image playerImage = Properties.Resources.galaga_ship;
        Image mediumImage = Properties.Resources.mediumEnemey;

        //lists
        //easy enemey 
        List<Rectangle> easyEnemey = new List<Rectangle>();
        List<int> easyYSpeed = new List<int>();
        List<int> easyXSpeed = new List<int>();
       
        List<Rectangle> bullet = new List<Rectangle>();
        
        List<Rectangle> easyBullet = new List<Rectangle>();

        //medium enemey 
        List<Rectangle> mediumEnemey = new List<Rectangle>();
        List<int> mediumXSpeed = new List<int>();
        List<int> mediumYSpeed = new List<int>();
        List<Rectangle> mediumBullet = new List<Rectangle>();


        //random gernerator 
        Random randGen = new Random();
        int randValue = 0;

        // keys 


        bool aDown = false;
        bool dDown = false;


        //paint 
        Rectangle player = new Rectangle(100, 800, 50, 50);
        Rectangle lifeCouunter1 = new Rectangle(50, 870, 50, 50);
        Rectangle lifeCouunter2 = new Rectangle(100, 870, 50, 50);
        Rectangle lifeCouunter3 = new Rectangle(150, 870, 50, 50);

        SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        Pen whitePen = new Pen(Color.Red);

        //sound 
        SoundPlayer musicPlayer = new SoundPlayer(Properties.Resources.bulletSound);

        public Form1()
        {
            InitializeComponent();


        }

        public void GameInitialize()
        {
            scoreLabel.Text = "";
            highSocreLabel.Text = "HighScore: ";
            gameTimer.Enabled = true;
            gameState = "running";
            timer = 0;
            score = 0;
            wave = 1;
            lives = 3;
            round = 1;
            easyEnemey.Clear();
            easyBullet.Clear();
            mediumBullet.Clear();
            mediumEnemey.Clear();
            easyXSpeed.Clear();
            easyYSpeed.Clear();
            mediumXSpeed.Clear();
            mediumYSpeed.Clear();
            
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                
                case Keys.A:
                    aDown = true;
                    break;
                case Keys.D:
                    dDown = true;
                    break;
                case Keys.Space:

                    if (gameState == "waiting" || gameState == "over")

                    {

                        GameInitialize();

                    }

                    break;

                case Keys.Escape:

                    if (gameState == "waiting" || gameState == "over")

                    {

                        Application.Exit();

                    }

                    break;
            }
        }

        

        private void gameTimer_Tick(object sender, EventArgs e)
        {

            //move player
            movePlayer();
            

            //move bullets
            moveBullets();



            //remove missing bullets 
            removeMissingBullets();

            //add easy enemey (wave one, round one)
            addRoundOne();

            //  add round 2
            addRoundTwo();


            //move easy enemeys round 1
            moveRoundOne();


            //move round  2
            moveRoundTwo();


            // spawn easy enemey bullets
            //move easy enemey bullets
            EasyBullets();


            //spawn medium enemey bullets 
            //move medium enemey bullets
            mediumBullets();




            //remove bullets that hit enemies and add points 
            enemeyCollisions();

            //player collisoins
            //easy enemey
            //medium enemey
            playerCollisions();

            
            //timer and score

            timer++;
            score++;
            scoreLabel.Text = $"{score}";
            
            //endgame 

            if (lives == 0)
            {
                gameTimer.Enabled = false;
                gameState = "over";

            }
            

            Refresh();
        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                
                case Keys.A:
                    aDown = false;
                    break;
                case Keys.D:
                    dDown = false;
                    break;


        }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
           
            e.Graphics.DrawImage(playerImage, player);
            



            //remove lives 
            if (lives == 3)
            {
                e.Graphics.DrawImage(playerImage, lifeCouunter1);
                e.Graphics.DrawImage(playerImage, lifeCouunter2);
                e.Graphics.DrawImage(playerImage, lifeCouunter3);
            }
            else if (lives == 2)
            {
                e.Graphics.DrawImage(playerImage, lifeCouunter1);
                e.Graphics.DrawImage(playerImage, lifeCouunter2);
                
            }
            else if (lives == 1)
            {
                e.Graphics.DrawImage(playerImage, lifeCouunter1);
                
            }
            


            for (int i = 0; i < bullet.Count(); i++)
            {
                e.Graphics.FillRectangle(redBrush, bullet[i]);
               
            }
            //easy enemey 
            for (int i = 0; i < easyEnemey.Count(); i++)
            {
                e.Graphics.DrawImage(easyImage, easyEnemey[i]);
                

            }

            for (int i = 0; i < easyBullet.Count(); i++)
            {
                e.Graphics.FillRectangle(redBrush, easyBullet[i]);

            }

            //medium enemey

            for (int i = 0; i < mediumEnemey.Count(); i++)
            {
                e.Graphics.DrawImage(mediumImage, mediumEnemey[i]);


            }

            for (int i = 0; i < mediumBullet.Count(); i++)
            {
                e.Graphics.FillRectangle(redBrush, mediumBullet[i]);

            }
            
            //gamestate 

            if (gameState == "waiting")

            {
                outputLabel.Text = "Galaga\n\nPress Space To Start\n\nPress Escape To Exit";
                highSocreLabel.Text = "HighScore:";
                scoreLabel.Text = "Score: 0000 ";

                

            }
            

            else if (gameState == "running")

            {
                outputLabel.Text = "";
                outputLabel.Visible = false;
                scoreLabel.Text = $"{score}";

            }
           

            

            else if (gameState == "over")

            {
                outputLabel.Visible = true;
                outputLabel.Text = $"Game Over\nYour Score Is {score}\nPress Space To Play Again\nPress Escape To Exit";
                


            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            bullet.Add(new Rectangle(player.X + 25, player.Y, 5, 15));
            SoundPlayer musicPlayer = new SoundPlayer(Properties.Resources.bulletSound);
            musicPlayer.Play();

        }


        public void movePlayer()
        {

            if (aDown == true && player.X > 0)
            {
                player.X -= playerSpeed; ;
            }

            if (dDown == true && player.X < this.Width - player.Width - 25)
            {
                player.X += playerSpeed;
            }

        }

        public void moveBullets()
        {
            for (int i = 0; i < bullet.Count(); i++)
            {

                int y = bullet[i].Y + bulletSpeed;


                bullet[i] = new Rectangle(bullet[i].X, y, 5, 20);
            }
        }

        public void removeMissingBullets()
        {
            for (int i = 0; i < bullet.Count(); i++)
            {

                if (bullet[i].Y < 0)
                {
                    bullet.RemoveAt(i);



                }
            }
        }

        public void addRoundOne()
        {


            if (timer == 300)
            {
                wave = 1;
                round = 1;
                easyEnemey.Add(new Rectangle(0, 0, 50, 50));
                easyXSpeed.Add(5);
                easyYSpeed.Add(5);



                for (int i = 1; i < 10; i++)
                {
                    easyEnemey.Add(new Rectangle(easyEnemey[i - 1].X - 100, easyEnemey[i - 1].Y - 100, 50, 50));
                    easyXSpeed.Add(5);
                    easyYSpeed.Add(5);
                }

            }

            //wave 2
            if (timer == 1000)
            {
                wave = 2;
                round = 1;
                easyEnemey.Add(new Rectangle(730, 730, 50, 50));
                easyXSpeed.Add(-5);
                easyYSpeed.Add(-5);

                for (int i = 1; i < 8; i++)
                {
                    easyEnemey.Add(new Rectangle(easyEnemey[i - 1].X + 100, easyEnemey[i - 1].Y + 100, 50, 50));
                    easyXSpeed.Add(-5);
                    easyYSpeed.Add(-5);

                }
            }

            //wave 3
            if (timer == 1300)
            {
                wave = 3;
                round = 1;
                easyEnemey.Add(new Rectangle(0, 200, 50, 50));
                easyXSpeed.Add(5);
                easyYSpeed.Add(0);

                for (int i = 1; i < 8; i++)
                {
                    easyEnemey.Add(new Rectangle(easyEnemey[i - 1].X - 100, easyEnemey[i - 1].Y + 0, 50, 50));
                    easyXSpeed.Add(5);
                    easyYSpeed.Add(0);

                }
            }

            //wave 4
            if (timer == 1800)
            {
                wave = 4;
                round = 1;
                easyEnemey.Add(new Rectangle(730, 730, 50, 50));
                easyXSpeed.Add(-5);
                easyYSpeed.Add(-5);

                for (int i = 1; i < 4; i++)
                {
                    easyEnemey.Add(new Rectangle(easyEnemey[i - 1].X + 100, easyEnemey[i - 1].Y + 100, 50, 50));
                    easyXSpeed.Add(-5);
                    easyYSpeed.Add(-5);

                }
            }
        }

        public void addRoundTwo()
        {
            //wave one
            if (timer == 2200)
            {
                round = 2;
                wave = 1;
                easyEnemey.Add(new Rectangle(0, 200, 50, 50));
                easyXSpeed.Add(5);
                easyYSpeed.Add(0);

                for (int i = 1; i < 8; i++)
                {
                    easyEnemey.Add(new Rectangle(easyEnemey[i - 1].X - 100, easyEnemey[i - 1].Y + 0, 50, 50));
                    easyXSpeed.Add(5);
                    easyYSpeed.Add(0);

                }
            }
            //wave 2
            if (timer == 2600)
            {
                round = 2;
                wave = 2;
                mediumEnemey.Add(new Rectangle(0, 600, 50, 50));
                mediumXSpeed.Add(7);
                mediumYSpeed.Add(7);

                for (int j = 1; j < 8; j++)
                {
                    mediumEnemey.Add(new Rectangle(mediumEnemey[j - 1].X - 100, mediumEnemey[j - 1].Y - 100, 50, 50));
                    mediumXSpeed.Add(7);
                    mediumYSpeed.Add(7);

                }
            }
        }

        public void moveRoundOne()
        {
            if (round == 1)
            {
                for (int i = 0; i < easyEnemey.Count(); i++)
                {

                    int y = easyEnemey[i].Y + easyYSpeed[i];
                    int x = easyEnemey[i].X + easyXSpeed[i];

                    easyEnemey[i] = new Rectangle(x, y, easyEnemey[i].Width, easyEnemey[i].Height);

                    if (wave == 1 && round == 1)
                    {
                        if (easyEnemey[i].X == 730 && easyEnemey[i].Y == 730)
                        {
                            easyXSpeed[i] = 0;
                            easyYSpeed[i] = -5;
                        }
                        else if (easyEnemey[i].Y == 100 && easyEnemey[i].X == 730)
                        {
                            easyXSpeed[i] = -5;
                            easyYSpeed[i] = 0;
                        }
                        else if (easyEnemey[i].X < 0 && easyEnemey[i].Y == 100)
                        {

                            easyEnemey.RemoveAt(i);
                            easyXSpeed.RemoveAt(i);
                            easyYSpeed.RemoveAt(i);

                        }
                    }
                    else if (wave == 2 && round == 1)
                    {
                        if (easyEnemey[i].Y < 0 && easyEnemey[i].X < 0)
                        {
                            easyEnemey.RemoveAt(i);
                            easyXSpeed.RemoveAt(i);
                            easyYSpeed.RemoveAt(i);
                        }
                    }
                    else if (wave == 3 && round == 1)
                    {
                        if (easyEnemey[i].Y == 200 && easyEnemey[i].X == 730)
                        {
                            easyXSpeed[i] = 0;
                            easyYSpeed[i] = 5;
                        }
                        else if (easyEnemey[i].Y == 400 && easyEnemey[i].X == 730)
                        {
                            easyXSpeed[i] = -5;
                            easyYSpeed[i] = 0;
                        }
                        else if (easyEnemey[i].Y == 400 && easyEnemey[i].X < 0)
                        {
                            easyEnemey.RemoveAt(i);
                            easyXSpeed.RemoveAt(i);
                            easyYSpeed.RemoveAt(i);
                        }
                    }
                    else if (wave == 4 && round == 1)
                    {
                        if (easyEnemey[i].Y < 0 && easyEnemey[i].X < 0)
                        {
                            easyXSpeed[i] = 5;
                            easyYSpeed[i] = 4;
                        }
                    }


                }
            }
        }

        public void moveRoundTwo()
        {
            if (round == 2)
            {
                for (int i = 0; i < easyEnemey.Count(); i++)
                {

                    for (int j = 0; j < mediumEnemey.Count(); j++)
                    {

                        int y = easyEnemey[i].Y + easyYSpeed[i];
                        int x = easyEnemey[i].X + easyXSpeed[i];
                        //int y = mediumEnemey[j].Y + mediumYSpeed[j];
                        //int x = mediumEnemey[j].X + mediumXSpeed[j];

                        easyEnemey[i] = new Rectangle(x, y, easyEnemey[i].Width, easyEnemey[i].Height);
                        //mediumEnemey[j] = new Rectangle(x, y, mediumEnemey[j].Width, mediumEnemey[j].Height);

                        if (wave == 1 && round == 2)
                        {
                            if (easyEnemey[i].Y == 200 && easyEnemey[i].X == 730)
                            {
                                easyXSpeed[i] = 0;
                                easyYSpeed[i] = -5;
                            }
                            else if (easyEnemey[i].Y == 400 && easyEnemey[i].X == 730)
                            {
                                easyXSpeed[i] = -5;
                                easyYSpeed[i] = 0;
                            }
                            else if (easyEnemey[i].Y == 400 && easyEnemey[i].X < 0)
                            {
                                easyEnemey.RemoveAt(i);
                                easyXSpeed.RemoveAt(i);
                                easyYSpeed.RemoveAt(i);
                            }
                        }
                        else if (wave == 2 && round == 2)
                        {
                            if (mediumEnemey[j].Y < 0 && mediumEnemey[j].X > 750)
                            {
                                mediumEnemey.RemoveAt(i);
                                mediumXSpeed.RemoveAt(i);
                                mediumYSpeed.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }

        public void EasyBullets()
        {
            randValue = randGen.Next(0, 101);

            if (randValue < 5 && easyEnemey.Count > 0)
            {
                //spawn easy bullets

                randValue = randGen.Next(0, easyEnemey.Count);

                int y = randGen.Next(easyEnemey[randValue].Y, easyEnemey[randValue].Y);
                int x = randGen.Next(easyEnemey[randValue].X, easyEnemey[randValue].X);

                easyBullet.Add(new Rectangle(x, y + 20, 5, 5));



            }
            //move easy enemey bullets
            for (int i = 0; i < easyBullet.Count(); i++)
            {
                int y = easyBullet[i].Y + easyBulletSpeed;
                easyBullet[i] = new Rectangle(easyBullet[i].X, y, 5, 20);
            }
        }

        public void mediumBullets()
        {
            //spawn medium enemey bullets 
            if (randValue < 5 && mediumEnemey.Count > 0)
            {
                randValue = randGen.Next(0, mediumEnemey.Count);

                int y = randGen.Next(mediumEnemey[randValue].Y, mediumEnemey[randValue].Y);
                int x = randGen.Next(mediumEnemey[randValue].X, mediumEnemey[randValue].X);

                mediumBullet.Add(new Rectangle(x, y + 20, 5, 5));

            }



            //move medium enemey bullets
            for (int i = 0; i < mediumBullet.Count(); i++)
            {
                int y = mediumBullet[i].Y + mediumBulletSpeed;
                mediumBullet[i] = new Rectangle(mediumBullet[i].X, y, 5, 20);
            }
        }

        public void playerCollisions()
        {
            //easy enemeys
            for (int i = 0; i < easyEnemey.Count(); i++)
            {
                if (easyEnemey[i].IntersectsWith(player))
                {
                    lives--;

                    easyEnemey.RemoveAt(i);

                }

                else if (lives == 0)
                {
                    gameState = "over";
                }
            }

            for (int i = 0; i < easyBullet.Count(); i++)
            {
                if (easyBullet[i].IntersectsWith(player))
                {
                    lives--;

                    easyBullet.RemoveAt(i);
                }
            }
            //medium enemey

            for (int i = 0; i < mediumEnemey.Count(); i++)
            {
                if (mediumEnemey[i].IntersectsWith(player))
                {
                    lives--;

                    mediumEnemey.RemoveAt(i);

                }

                else if (lives == 0)
                {
                    gameState = "over";
                }
            }

            for (int i = 0; i < mediumBullet.Count(); i++)
            {
                if (mediumBullet[i].IntersectsWith(player))
                {
                    lives--;

                    mediumBullet.RemoveAt(i);
                }
            }
        }

        public void enemeyCollisions()
        {
            //remove bullets that hit enemies and add points 
            for (int i = 0; i < bullet.Count(); i++)
            {
                for (int j = 0; j < easyEnemey.Count(); j++)
                {

                    if (bullet[i].IntersectsWith(easyEnemey[j]))
                    {
                        //remove bullet i and enemey j
                        bullet.RemoveAt(i);


                        easyEnemey.RemoveAt(j);
                        easyXSpeed.RemoveAt(j);
                        easyYSpeed.RemoveAt(j);

                        score = score + 200;
                        SoundPlayer musicPlayer = new SoundPlayer(Properties.Resources.enemeyDead);
                        musicPlayer.Play();
                        break;
                    }
                }
            }
        }
    }


    

}