﻿//--------------------------------------------------------------------
//  Kubeah ! Open Source Project
//  
//  Kubeah Chat
//  Version 1.4.1 Final
//--------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace ChatLocalClient
{

    public partial class frmMain : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;

        public frmMain()
        {
            InitializeComponent();
            //==============================LorsDuDémmarage===================================================
            lblDescription2.Visible = false; //LABEL DESCRIPTION 2 non visible
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//Création d'un socket
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            lblIPPersonnel.Text = GetLocalIP();//Met l'adresse ip du pc dans le label
            tbxMessageEnvoit.MaxLength = 72;//Limite le nombre de caractères de la tbx à 32 caractères
            pbxLogoPetit.Visible = false;//Permet de ne pas voir le logo au fond à droit
            NomDestinataireToolStripMenuItem.Visible = false;
            NomDestinataireToolStripMenuItem.Text = lblIPPersonnel.Text;
            iPPersonnelToolStripMenuItem.Visible = false;
            this.MaximizeBox = false;//Permet de ne pas pouvoir utiliser le bouton pour maximiser la fenêtre
            //GESTIONFOCUS====================================================================================
            timContrôleFocus.Enabled = true;//Départ du timer focus "Focus sur listbox"
            oNToolStripMenuItem.Checked = true;
            lblNomPCDest.Visible = false;
            lblEtatPing.Visible = false;
            //================================================================================================
            IPSeparationString(lblIPPersonnel.Text);
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "172.0.0.1";
        }



        //Fonction ENVOIDUMESSAGE===============================================================================================/
        //======================================================================================================================/
        // 1 = OUI
        // 0 = NON
        // Pour l'affichage dans la TBX
        public void EnvoiDuMessage(string lblTextEnvoi, int iAffichageTBX)
        {
            try
            {
                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();//Encode en UTF8 (préparationn du packet)
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(lblTextEnvoi);

                sck.Send(msg);
                if(iAffichageTBX == 1)
                {
                    lbxTchat.Items.Add("Moi : " + tbxMessageEnvoit.Text);
                    tbxMessageEnvoit.Clear();
                }
                else
                {
                    //Ne fait rien
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //Fonction LABELTOINTTEST==============================================================================================/
        //====================================================================================================================/
        //Return : 1 = Erreur 2 = PASS
        //Permet de contrôler si les v
        public int LabelToIntTest(string sText, int i)
        {
            try
            {
                int iVerification = Convert.ToInt32(sText);
                return i = 2;
            }
            catch
            {
                return i = 1;
            }
        }
        //Fonction NAMEMACHINTEWITHIP========================================================================================/
        //===================================================================================================================/
        //Return : Detinataire Name
        private static string NameMachineWithIP(string ipAdress)
        {
            string machineName = string.Empty;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAdress);
                machineName = hostEntry.HostName;
            }
            catch
            {
                //MessageBox.Show("Destinataire introuvable");
            }
            return machineName;
        }
        //FONCTION PINGDEST====================================================================================================/
        //=====================================================================================================================/
        //Permet de réaliser un ping avec un IP donnée
        public static bool PingDest(string sIpAdress)
        {
            bool bResult;
            Ping ping = new Ping();
            PingReply pingresult = ping.Send(sIpAdress, 60);
            if (pingresult.Status.ToString() == "Success")
            {
                return bResult = true;
            }
            else
            {
                return bResult = false;
            }
        }
        //Fonction SPLITSTRING=================================================================================================/
        //=====================================================================================================================/
        //Permet l'affichage de l'IP séparé dans les textbox
        public void IPSeparationString(string sIP)
        {
            string[] stringSeparators = new string[] { "." };
            string[] result;

            // ...
            result = sIP.Split(stringSeparators, StringSplitOptions.None);

            string sIp1 = result[0];
            string sIp2 = result[1];
            string sIp3 = result[2];

            tbxIP1.Text = sIp1;
            tbxIP2.Text = sIp2;
            tbxIP3.Text = sIp3;
        }
        //=====================================================================================================================/
        //====================================================================================================================/


        //=====================BTNSTART=============================================================================
        private void btnSart_Click(object sender, EventArgs e)
        {
            lblPatience.Visible = true;
            lblEtatPing.Visible = false;
            lblNomPCDest.Visible = false;
            this.Enabled = false;
            string sIPDestinataire = tbxIP1.Text + "." + tbxIP2.Text + "." + tbxIP3.Text + "." + tbxIP4.Text;
            if (sIPDestinataire == lblIPPersonnel.Text) { tbxIP4.BackColor = Color.Red; }
            if (tbxIP1.BackColor != Color.Red)
                if (tbxIP2.BackColor != Color.Red)
                {
                    if (tbxIP3.BackColor != Color.Red)
                    {
                        if (tbxIP4.BackColor != Color.Red)
                        {
                            if (tbxIP4.BackColor != Color.PaleGreen)
                            {
                                lblEtatPing.Visible = true;
                                string sNameDestinataire = NameMachineWithIP(sIPDestinataire);
                                bool bResultPing = PingDest(sIPDestinataire);
                                if (bResultPing == true)
                                {
                                    lblEtatPing.Text = "Ping : OK";
                                    lblEtatPing.ForeColor = Color.Green;
                                    if (btnSart.Text == "Vérifier l'IP")
                                    {
                                        if (sNameDestinataire == "")
                                        {
                                            lblNomPCDest.Text = "Nom :" + "\r\n" + "Introuvable";
                                            lblNomPCDest.ForeColor = Color.Red;
                                            lblNomPCDest.Visible = true;
                                            tbxIP4.BackColor = Color.Red;
                                        }
                                        else
                                        {
                                            btnSart.Text = "Commencer";
                                            lblNomPCDest.Visible = true;
                                            lblNomPCDest.Text = "Nom :" + "\r\n" + sNameDestinataire;
                                            lblNomPCDest.ForeColor = Color.Black;
                                            bResultPing = PingDest(sIPDestinataire);
                                        }
                                    }
                                    else
                                    {
                                        //___________MinimiserLaFenetre________________
                                        this.Width = 620;
                                        lblDescription.Visible = false;
                                        btnSart.Visible = false;
                                        lblDescription2.Visible = true;
                                        lblIPDESTINATAIRE.Visible = false;
                                        pbxLogo1.Visible = false;
                                        pbxLogoPetit.Visible = true;
                                        lblIPPersonnel.Visible = false;
                                        lblFixeCePC.Visible = false;
                                        NomDestinataireToolStripMenuItem.Visible = true;
                                        tbxIP1.Visible = false;
                                        tbxIP2.Visible = false;
                                        tbxIP3.Visible = false;
                                        tbxIP4.Visible = false;
                                        NomDestinataireToolStripMenuItem.Text = "Destinataire : " + sNameDestinataire;
                                        lblNomPCDest.Visible = false;
                                        iPPersonnelToolStripMenuItem.Text = "Mon IP : " + lblIPPersonnel.Text;
                                        iPPersonnelToolStripMenuItem.Visible = true;
                                        //______________________________________________
                                        try
                                        {
                                            epLocal = new IPEndPoint(IPAddress.Parse(lblIPPersonnel.Text), 3056);//Utilise le port 3056
                                            sck.Bind(epLocal);

                                            epRemote = new IPEndPoint(IPAddress.Parse(sIPDestinataire), 3056);
                                            sck.Connect(epRemote);

                                            byte[] buffer = new byte[1500];
                                            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageEnvoi), buffer);

                                            btnSart.Text = "Connecté";
                                            btnSart.Enabled = false;
                                            btnEnvoi.Enabled = true;
                                            tbxMessageEnvoit.Focus();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.ToString());
                                        }
                                        //Envoit la clé à l'autre client pour connecté
                                        EnvoiDuMessage("tuiFZCz56786casdcssdcvuivgboRTSDetre67Rz7463178", 0);//tuiFZCz56786casdcssdcvuivgboRTSDetre67Rz7463178->Clé Présent
                                    }
                                }
                                else
                                {
                                    lblEtatPing.Text = "Ping : Echoué";
                                    lblEtatPing.ForeColor = Color.Red;
                                    lblNomPCDest.Visible = false;
                                    tbxIP4.BackColor = Color.Red;
                                }
                            }
                        }
                    }
                }
            this.Enabled = true;
            lblPatience.Visible = false;
        }
        //==========================================================================================================================================================



        //====================================BTNEnvoi====================================================
        private void btnEnvoi_Click(object sender, EventArgs e)
        {
            if (btnSart.Visible == true)
            {

            }
            else
            {
                if (tbxMessageEnvoit.Text == "")
                {
                  //Permet de ne rien envoyer si la texte box est vide
                }
                else
                {
                    if (lblStatutAmi.Text == "Ami : " || lblStatutAmi.Text == "Ami : Déconnecté")
                    {
                        MessageBox.Show("Votre destinataire n'est pas connecté." + "\r\n" + "!!! Si le destinataire n'est pas connecté, il ne reçoit pas les messages et ne pourra pas les consulter ultérieurement." + "\r\n" + "Attendez qu'il se reconnecte.");
                    }
                    else
                    {
                    EnvoiDuMessage(tbxMessageEnvoit.Text, 1);
                    }
                }
             }
        }
        //===============================================================================================

        //======================================GestionFocus==============================================
        private void oNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Gestion du Focus - Dans la barre du menu
            oNToolStripMenuItem.Checked = true;
            oFFToolStripMenuItem.Checked = false;
            timContrôleFocus.Enabled = true;//Départ du timer focus
        }

        private void oFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            oFFToolStripMenuItem.Checked = true;
            oNToolStripMenuItem.Checked = false;
            timContrôleFocus.Enabled = false;
        }
        private void timContrôleFocus_Tick(object sender, EventArgs e)
        {
            lbxTchat.SelectedIndex = lbxTchat.Items.Count - 1;
        }
        //=====================================FINGestionFocus==============================================

        //MENU================================================MENU==============================MENU========================================================
        private void arrêterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();//Menu 
        }

        private void redémmarerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();//Redémmare le pc
        }

        private void aProposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Kubeah Chat - V.1.4.1" + "\r\n" + "Kubeah! The Open Source Project" + "\r\n" + "\r\n" + "You want to join the developer team?" + "\r\n" + "Contact : support@kubeah.com" + "\r\n" + "\r\n" + "www.kubeah.com");
        }

        private void siteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://sites.google.com/view/kubeahchat");//Ouvre le lien dans le navigateur par défault
            }
            catch { }
        }
        //FINMENU==============================================FIN============================================MENU===========================================

        //Gestion des informations utilisateur IP destinataire===============================================================================================
        private void tbxIP1_TextChanged(object sender, EventArgs e)
        {
            lblEtatPing.Visible = false;
            lblNomPCDest.Visible = false;
            int iTemp = LabelToIntTest(tbxIP1.Text, 1);//Utilise la fonction "LabelToIntTest"
            if (iTemp == 2){tbxIP1.BackColor = Color.Snow;} else{tbxIP1.BackColor = Color.Red;}
            if (tbxIP1.Text == "0") { tbxIP1.BackColor = Color.Red; }
            btnSart.Text = "Vérifier l'IP";//Changement du texte dans le btn
            try { int iTemp2 = Convert.ToInt32(tbxIP1.Text); if (iTemp2 > 255 || iTemp2 < 0 || iTemp2 == -0) { tbxIP1.BackColor = Color.Red; } } catch { } //Doit être supérieur à 0 mais inférieur à 256

        }
        private void tbxIP2_TextChanged(object sender, EventArgs e)
        {
            lblEtatPing.Visible = false;
            lblNomPCDest.Visible = false;
            int iTemp = LabelToIntTest(tbxIP2.Text, 1);//Utilise la fonction "LabelToIntTest"
            if (iTemp == 2) { tbxIP2.BackColor = Color.Snow; } else { tbxIP2.BackColor = Color.Red; }
            btnSart.Text = "Vérifier l'IP";//Changement du texte dans le btn
            try { int iTemp2 = Convert.ToInt32(tbxIP2.Text); if (iTemp2 > 255 || iTemp2 < 0) {tbxIP2.BackColor = Color.Red;}}catch { } //Doit être supérieur ou égal à 0 mais inférieur à 256
        }
        private void tbxIP3_TextChanged(object sender, EventArgs e)
        {
            lblEtatPing.Visible = false;
            lblNomPCDest.Visible = false;
            int iTemp = LabelToIntTest(tbxIP3.Text, 1);//Utilise la fonction "LabelToIntTest"
            if (iTemp == 2){tbxIP3.BackColor = Color.Snow;} else{tbxIP3.BackColor = Color.Red;}
            btnSart.Text = "Vérifier l'IP";//Changement du texte dans le btn
            try { int iTemp2 = Convert.ToInt32(tbxIP3.Text); if (iTemp2 > 255 || iTemp2 < 0) { tbxIP3.BackColor = Color.Red; } } catch { } //Doit être supérieur ou égal à 0 mais inférieur à 256
        }

        private void tbxIP4_TextChanged(object sender, EventArgs e)
        {
            lblEtatPing.Visible = false;
            lblNomPCDest.Visible = false;
            int iTemp = LabelToIntTest(tbxIP4.Text, 1);//Utilise la fonction "LabelToIntTest"
            if (iTemp == 2){tbxIP4.BackColor = Color.Snow;} else{tbxIP4.BackColor = Color.Red;}
            btnSart.Text = "Vérifier l'IP";//Changement du texte dans le btn
            try { int iTemp2 = Convert.ToInt32(tbxIP4.Text); if (iTemp2 > 255 || iTemp2 < 0 || iTemp2 == -0) { tbxIP4.BackColor = Color.Red; } } catch { } //Doit être supérieur à 0 mais inférieur à 256
        }
        //======================================FIN========================FIN===============================================================================
        private void tbxMessageEnvoit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//Quand appuie sur touche enter
            {
                e.SuppressKeyPress = true;
                if (btnSart.Visible == true)
                {

                }
                else
                {
                    if (tbxMessageEnvoit.Text == "")
                    {
                        //Permet de ne rien envoyer si la texte box est vide
                    }
                    else
                    {
                        if (lblStatutAmi.Text == "Ami : " || lblStatutAmi.Text == "Ami : Déconnecté")
                        {
                            MessageBox.Show("Votre destinataire n'est pas connecté." + "\r\n" + "!!! Si l'utilisateur n'est pas connecté il ne recevra pas les messages et il ne pourra pas les consulter ultérieurement." + "\r\n" + "Attendez qu'il se reconnecte.");
                        }
                        else
                        {
                            EnvoiDuMessage(tbxMessageEnvoit.Text, 1);
                        }
                    }
                }
            }
        }

        //GestionNbrCactèresRestants=======================================================================================
        private void timNbrCaractères_Tick(object sender, EventArgs e)
        {
            //Permet de montrer le nombre de caractères restants
            int iNbrCaract = tbxMessageEnvoit.TextLength;//.TextLength permet de donner le nombre de caractères
            iNbrCaract = 72 - iNbrCaract;//Calcul pour donner le nombre de caractères restants
            lblNbrCaractRestants.Text = Convert.ToString(iNbrCaract);//Permet l'affichage de iNbrCract
        }
        //FINGestionNbrCaractères===================================================================FIN====================
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)//Quand l'utilisateur ferme la fenêtre
        {
            //Envoi la clé pour dire à l'autre client qu'il est absent
            //Que si la conversation à démmarée
            if (btnSart.Visible == false)
            {
                EnvoiDuMessage("789ZCFZTiniwjZTUvjkas79012798", 0);//789ZCFZTiniwjZTUvjkas79012798->Clé Absence
            }
        }
        //=====================FonctionReceptionMessage=====================================================================
        // Le code pour la méthode(ami connecté/Déconnecté) est intégré directement dans la fonction
        // d'envoi du message. Elle risque d'évoluer et de changer de place. 
        // Les clé sont utilisées dans ce cas comme moyen de comparaison grâce à des "IF". !!!!!Il est possible de changer
        // l'état du client en envoyant la clé par message.
        //
        // Méthode(ami connecté/Déconnecté):
        // client 1 se connecte -> Il envoit la clé(dans le vide)-> client 2 se connecte -> Il envoit la clé ->
        // Client 1 reçoit, compare le message -> client 1 regarde contenu lblStatutAmi(execute selon condition) ->
        // client 2 reçoit, compare le message -> client 2 regarde contenu lblStatutAmi(execute selon condition)
        private void MessageEnvoi(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] receivedData = new byte[1464];

                    receivedData = (byte[])aResult.AsyncState;

                    UTF8Encoding enc = new UTF8Encoding();
                    string receivedMessage = enc.GetString(receivedData);
                    //Comparaison chaine de caractère reçu
                    if (receivedMessage == "789ZCFZTiniwjZTUvjkas79012798\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0")
                    {
                        lblStatutAmi.Text = "Ami : Déconnecté";
                        lblStatutAmi.ForeColor = Color.Red;
                    }
                    else
                    {
                        //Comparaison chaine de caractère reçu est regarde contenu lblStatutAmi
                        if (receivedMessage == "tuiFZCz56786casdcssdcvuivgboRTSDetre67Rz7463178\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0" && lblStatutAmi.Text == "Ami : " || lblStatutAmi.Text == "Ami : Déconnecté")
                        {
                            lblStatutAmi.Text = "Ami : Connecté";//Changement du statut le la personne
                            lblStatutAmi.ForeColor = Color.Green;//Changement de la couleur du text
                            //Pour confirmation que l'autre utilisateur ai dans le label Connecté
                            EnvoiDuMessage("tuiFZCz56786casdcssdcvuivgboRTSDetre67Rz7463178", 0);
                        }
                        else
                        {
                            if (receivedMessage == "tuiFZCz56786casdcssdcvuivgboRTSDetre67Rz7463178\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0")
                            {

                            }
                            else
                            {
                                lbxTchat.Items.Add("Ami :      " + receivedMessage);
                            }
                        }
                    }
                }

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageEnvoi), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
//==========================FIN========================INFOS==================================================
//tuiFZCz56786casdcssdcvuivgboRTSDetre67Rz7463178-> Clé Présent
//789ZCFZTiniwjZTUvjkas79012798-> Clé Absent
//© Kubeah !