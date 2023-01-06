using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Data.OleDb;
using System.Timers;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace bot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        public static string connectString2 = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath.ToString().Remove(Application.StartupPath.ToString().Length - 30, 30) + "\\databases\\test.mdb";
        private OleDbConnection myConnection = new OleDbConnection(connectString2);
        private System.Timers.Timer aTimer;
        private System.Timers.Timer aTimer2;
        public static int otvet = 0;
        public static int nagrada = 0;
        public static int kmn_otvet = 0;
        public static bool pos = false;
        public Random r = new Random();
        public static List<string> steamids = new List<string>();
        public static List<int> experience = new List<int>();
        public static List<string> igroki = new List<string>();

        [Command("ответ")]
        public async Task Otvet(string otvet2 = null)
        {
            //Console.WriteLine(otvet + " " + otvet2);
            myConnection.Open();
            try
            {
                if (otvet2 != null && int.Parse(otvet2) == otvet && pos == true && Context.Message.Channel == Context.Guild.GetChannel(803312167998062633))
                {
                    pos = false;
                    int money = int.Parse(new OleDbCommand("SELECT d_money FROM Test WHERE d_dis = '" + Context.User.Id + "'", myConnection).ExecuteScalar().ToString());
                    await new OleDbCommand("UPDATE Test SET d_money = " + (money + nagrada).ToString() + " WHERE d_dis = '" + Context.User.Id + "'", myConnection).ExecuteNonQueryAsync();
                    await ReplyAsync("``Правильный ответ! Вы заработали:`` " + nagrada.ToString() + " ``лека``");
                }
            }
            catch { };
            myConnection.Close();
        }

        [Command("бан")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "У вас нету полномочий чтобы забанить ``ban_member``!")]
        public async Task BanMember(IGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                await ReplyAsync("``Выберите существующего пользователя!``");
                return;
            }
            if (reason == null) reason = "Не указана!";

            await Context.Guild.AddBanAsync(user, 1, reason);

            var EmbedBuilder = new EmbedBuilder()
                .WithDescription($":white_check_mark: {user.Mention} был забанен\n**Reason** {reason}")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("User Ban Log")
                    .WithIconUrl("https://i.imgur.com/6Bi17B3.png");
                });
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);

            ITextChannel logChannel = Context.Client.GetChannel(622396063369789481) as ITextChannel;
            var EmbedBuilderLog = new EmbedBuilder()
                .WithDescription($"{user.Mention} был забанен\n**Reason** {reason}\n**Moderator** {Context.User.Mention}")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("User Ban Log")
                    .WithIconUrl("https://i.imgur.com/6Bi17B3.png");
                });
            Embed embedLog = EmbedBuilderLog.Build();
            await logChannel.SendMessageAsync(embed: embedLog);

        }

        [Command("матстарт")]
        [RequireOwner]
        public async Task mathstart()
        {
            try
            {
                r = new Random();
                aTimer = new System.Timers.Timer(120000);
                aTimer.Elapsed += async (sender, e) => await maths();
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                aTimer.Start();
                await ReplyAsync("``Математический бот запущен``");
            }
            catch (Exception ec)
            {
                Console.WriteLine(ec);
            }
        }

        [Command("синхстарт")]
        [RequireOwner]
        public async Task synhstart()//старт синхронизации с игровым сервером
        {
            try
            {
                r = new Random();
                aTimer2 = new System.Timers.Timer(300000);
                aTimer2.Elapsed += async (sender, e) => await obnova();
                aTimer2.AutoReset = true;
                aTimer2.Enabled = true;
                aTimer2.Start();
                await ReplyAsync("``Синхронизирующий бот запущен``");
            }
            catch (Exception ec)
            {
                Console.WriteLine(ec);
            }
        }

        [Command("синхстоп")]//окончание синхронизации с игровым сервером
        [RequireOwner]
        public async Task synhstop()
        {
            try
            {
                aTimer.Stop();
                aTimer.Dispose();
                await ReplyAsync("``Синхронизирующий бот выключен``");
            }
            catch { };
        }

        private async Task maths()
        {
            int a = r.Next(0, 1001);
            int b = r.Next(0, 1001);
            pos = true;
            nagrada = r.Next(1, 6);
            var chnl = Context.Guild.GetChannel(882958028050137108) as IMessageChannel;
            if (r.Next(0, 2) == 0)
            {
                otvet = a - b;
                await chnl.SendMessageAsync("```" + a.ToString() + " - " + b.ToString() + " = ?```");
            }
            else
            {
                otvet = a + b;
                await chnl.SendMessageAsync("```" + a.ToString() + " + " + b.ToString() + " = ?```");
            }
            await Task.CompletedTask;
            await chnl.SendMessageAsync("``Для ответа напишите &ответ [ваш числовой ответ]``");
        }

        [Command("кмн")]//миниигра камень-ножницы-бумага
        public async Task kmn(string kanobu = null)
        {
            try
            {
                if (kanobu != null && Context.Message.Channel == Context.Guild.GetChannel(882957800400093244))
                {
                    int ot = r.Next(0, 3);
                    int chestno = r.Next(0, 3);
                    if (chestno == 0)
                    {
                        switch (kanobu)
                        {
                            case "камень":
                                await ReplyAsync("``Бумага``\n``Вы проиграли!``");
                                break;
                            case "ножницы":
                                await ReplyAsync("``Камень``\n``Вы проиграли!``");
                                break;
                            case "бумага":
                                await ReplyAsync("``Ножницы``\n``Вы проиграли!``");
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (kanobu)
                        {
                            case "камень":
                                switch (ot)
                                {
                                    case 0:
                                        await ReplyAsync("``Камень``\n``Ничья!``");
                                        break;
                                    case 1:
                                        await ReplyAsync("``Ножницы``\n``Вы выиграли!``");
                                        break;
                                    default:
                                        await ReplyAsync("``Бумага``\n``Вы проиграли!``");
                                        break;
                                }
                                break;
                            case "ножницы":
                                switch (ot)
                                {
                                    case 0:
                                        await ReplyAsync("``Камень``\n``Вы проиграли!``");
                                        break;
                                    case 1:
                                        await ReplyAsync("``Ножницы``\n``Ничья!``");
                                        break;
                                    default:
                                        await ReplyAsync("``Бумага``\n``Вы выиграли!``");
                                        break;
                                }
                                break;
                            case "бумага":
                                switch (ot)
                                {
                                    case 0:
                                        await ReplyAsync("``Камень``\n``Вы выиграли!``");
                                        break;
                                    case 1:
                                        await ReplyAsync("``Ножницы``\n``Вы проиграли!``");
                                        break;
                                    default:
                                        await ReplyAsync("``Бумага``\n``Ничья!``");
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch { }
        }

        async Task<string> ReadTextAsync(string filePath)
        {
            var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read,
                    bufferSize: 4096, useAsync: true);

            var sb = new StringBuilder();

            byte[] buffer = new byte[0x1000];
            int numRead;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return sb.ToString();
        }

        private async Task obnova()
        {
            try
            {
                experience.Clear();
                steamids.Clear();
                igroki.Clear();
                foreach (var temp in new DirectoryInfo(@"C:\Users\Artem\Desktop\playersdata").GetFiles())
                {
                    var name = Path.GetFileNameWithoutExtension(temp.FullName);
                    string text = await ReadTextAsync(temp.FullName);
                    string[] data = text.Split('|');
                    if (igroki.Contains(name) == false && data.Length > 0)
                    {
                        igroki.Add(name);
                        experience.Add(int.Parse(data[0]));
                        string g = data[1];
                        if (g.Length > 1)
                        {
                            steamids.Add(g.Replace("@steam", ""));
                        }
                        else
                        {
                            steamids.Add(g);
                        }
                    }
                }
                myConnection.Open();
                for (int i = 0; i < igroki.Count; i++)
                {
                    if (await new OleDbCommand("SELECT d_steam FROM Test WHERE d_steam = '" + steamids[i] + "'", myConnection).ExecuteScalarAsync() != null && steamids[i] != "0")
                    {
                        int money = int.Parse(new OleDbCommand("SELECT d_money FROM Test WHERE d_steam = '" + steamids[i] + "'", myConnection).ExecuteScalar().ToString());
                        int exp = int.Parse(new OleDbCommand("SELECT d_exp FROM Test WHERE d_steam = '" + steamids[i] + "'", myConnection).ExecuteScalar().ToString());
                        if (experience[i] < 100 && exp >= 100)
                        {
                            money += 20; 
                        }
                        if (experience[i] < 200 && exp >= 200)
                        {
                            money += 30;
                        }
                        if (experience[i] < 400 && exp >= 400)
                        {
                            money += 50;
                        }
                        if (experience[i] < 800 && exp >= 800)
                        {
                            money += 80;
                        }
                        if (experience[i] < 1600 && exp >= 1600)
                        {
                            money += 130;
                        }
                        if (experience[i] < 3200 && exp >= 3200)
                        {
                            money += 210;
                        }
                        if (experience[i] < 6400 && exp >= 6400)
                        {
                            money += 340;
                        }
                        if (experience[i] < 12800 && exp >= 12800)
                        {
                            money += 550;
                        }
                        if (experience[i] < 25600 && exp >= 25600)
                        {
                            money += 890;
                        }
                        if (experience[i] < 51200 && exp >= 51200)
                        {
                            money += 1440;
                        }
                        if (experience[i] < 102400 && exp >= 102400)
                        {
                            money += 2330;
                        }
                        if (experience[i] < 204800 && exp >= 204800)
                        {
                            money += 3770;
                        }
                        OleDbCommand um = new OleDbCommand("UPDATE Test SET d_money = " + money + " WHERE d_steam = '" + steamids[i] + "'", myConnection);
                        await um.ExecuteNonQueryAsync();
                        await new OleDbCommand("UPDATE Test SET d_exp = " + experience[i] + " WHERE d_steam = '" + steamids[i] + "'", myConnection).ExecuteNonQueryAsync();
                    }
                }
                myConnection.Close();
            }
            catch (Exception ec)
            {
                Console.WriteLine(ec);
            }
            await Task.CompletedTask;
        }

        [Command("матстоп")]
        [RequireOwner]
        public async Task mathstop()
        {
            try
            {
                aTimer.Stop();
                aTimer.Dispose();
                await ReplyAsync("``Математический бот выключен``");
            }
            catch { };
        }

        [Command("статус")]
        [RequireOwner]
        public async Task status([Remainder] string message = null)
        {
            if (message != null)
            {
                await Context.Client.SetActivityAsync(new Game(message, ActivityType.Playing));
                await ReplyAsync("``Статус включён``");
            }
        }

        /*[Command("онлайн")]
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "``Не трогай!``")]
        public async Task Onlain()
        {
            SocketRole role = Context.Guild.GetRole(585767086815576069);
            foreach (var ig in Context.Guild.Users)
            {
                if (Context.Guild.GetUser(ig.Id).Roles.Contains(Context.Guild.GetRole(585767086815576069)) == true && Context.Guild.GetUser(ig.Id).Roles.Contains(Context.Guild.GetRole(588343962675183636)) == false && Context.Guild.GetUser(ig.Id).Roles.Contains(Context.Guild.GetRole(785165602771828766)) == false)
                {
                    try
                    {
                        await ig.SendMessageAsync(ig.Mention + " ``быстро пошёл онлайнить на игровой сервер!``");
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            await ReplyAsync(role.Mention + " ``быстро онлайнить!``");
        }*/

        [Command("рулетка")]
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "``Детям пушка не игрушка!``")]
        public async Task Ruletka(string us = null, [Remainder] string reason = null)
        {
            try
            {
                Console.WriteLine(us);
                us = us.Replace("<@!","");
                us = us.Replace(">", "");
                Console.WriteLine(us);
                var urs = us.Split('|');
                List<IGuildUser> users = new List<IGuildUser>();
                if (urs != null && urs.Length > 0 && Context.Message.Channel == Context.Guild.GetChannel(882978872189145088) && reason != null)
                {
                    foreach (var ig in urs)
                    {
                        users.Add(Context.Guild.GetUser(ulong.Parse(ig)));
                    }
                    string nakaz = reason;
                    int victim = r.Next(0, users.Count);
                    await ReplyAsync(users[victim].Mention + " ``стал жертвой наказания: ``" + nakaz);
                }
            }
            catch(Exception er)
            {
                Console.WriteLine(er.ToString());
            }
        }

        [Command("updus")]
        [RequireOwner()]
        public async Task Updus()
        {
            myConnection.Open();
            foreach (var us in Context.Guild.Users)
            {
                if (us.IsBot == false && us.Roles.Contains<SocketRole>(Context.Guild.GetRole(716606431578685502)))
                {
                    if (await new OleDbCommand("SELECT d_dis FROM Test WHERE d_dis = '" + us.Id.ToString() + "'", myConnection).ExecuteScalarAsync() == null)
                    {
                        Console.WriteLine("успешно");
                        string query = "INSERT INTO Test(d_dis, d_money, d_exp) VALUES('" + us.Id + "', 10, 0)";
                        OleDbCommand command = new OleDbCommand(query, myConnection);
                        await command.ExecuteNonQueryAsync();
                    }
                    /*if (await new OleDbCommand("SELECT d_dis FROM Test WHERE d_dis = '" + us.Id.ToString() + "'", myConnection).ExecuteScalarAsync() != null)
                    {
                        Console.WriteLine("yes");
                        string query = "UPDATE Test SET d_exp = 0";
                        OleDbCommand command = new OleDbCommand(query, myConnection);
                        await command.ExecuteNonQueryAsync();
                    }*/
                }
            }
            List<string> ids = new List<string>();
            foreach (var us in Context.Guild.Users)
            {
                if (us.IsBot == false && us.Roles.Contains<SocketRole>(Context.Guild.GetRole(716606431578685502)))
                {
                    ids.Add(us.Id.ToString());
                }
            }
            OleDbCommand command2 = new OleDbCommand("SELECT d_dis FROM Test", myConnection);
            OleDbDataReader id = command2.ExecuteReader();
            while (id.Read())
            {
                var i = id[0].ToString();
                if (ids.Contains(i) == false)
                {
                    await new OleDbCommand("DELETE FROM Test WHERE d_dis = '" + i.ToString() + "'", myConnection).ExecuteNonQueryAsync();
                }
            }
            id.Close();
            myConnection.Close();
            await ReplyAsync("``Успешно``");
        }

        [Command("синх")]
        public async Task sync(string steamid = null)
        {
            if (steamid == null)
            {
                await ReplyAsync("``Введите steamid64!``");
                return;
            }
            myConnection.Open();
            if (Context.Guild.GetUser(Context.User.Id).Roles.Contains(Context.Guild.GetRole(716606431578685502)) == true && await new OleDbCommand("SELECT d_dis FROM Test WHERE d_steam = '" + steamid + "'", myConnection).ExecuteScalarAsync() == null)
            {
                await new OleDbCommand("UPDATE Test SET d_steam = '" + steamid + "' WHERE d_dis = '" + Context.User.Id + "'", myConnection).ExecuteNonQueryAsync();
                await ReplyAsync("``Успешно``");
                ITextChannel logChannel = Context.Client.GetChannel(622396063369789481) as ITextChannel;
                await logChannel.SendMessageAsync(Context.User.Mention + " синхронизировал свой steamid64.");
            }
            else
            {
                await ReplyAsync("``Этот steamid уже зарегистрирован!``");
            }
            myConnection.Close();
        }

        [Command("помощь")]
        public async Task help()
        {
            await ReplyAsync("``&помощь`` - все доступные вам команды для бота\n``&кмн [камень|ножницы|бумага]`` - работает только в канале <#882957800400093244>\n``&рулетка @Игрок1|@Игрок2|@Игрок3 [наказание]`` - русская рулетка\n```Скоро будут добавлены новые команды...```");
        }

        [Command("админпомощь")]
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "``У вас нет полномочий!``")]
        public async Task adminhelp()
        {
            await ReplyAsync("``&админпомощь`` - все доступные только админам команды для бота\n``&updus`` - ещё раз синхронизирует базу данных и игроков на сервере ||(выполняется довольно долго)||\n``&балансигрок @Игрок`` - баланс лека выбранного игрока\n``&балансобн @Игрок [число лека, которое необходимо добавить или снять]``- изменение баланса лека выбранного игрока\n``&лвлигрок @Игрок`` - ранг выбранного игрока в scp\n``&варндискорд`` - варн игрока за нарушение правил дискорда (роль выдаётся автоматически)\n``&варндонат`` - варн игрока за нарушение правил донатеров (роль выдаётся автоматически)\n``&варнигра`` - варн игрока за нарушение правил игрового сервера в scp (роль выдаётся автоматически)\n``&собрание`` - призыв админов на собрание\n``&онлайн`` - призыв админов на игровой сервер \n``&рулетка @Игрок1|@Игрок2|@Игрок3 [наказание]`` - русская рулетка");
        }

        [Command("балансобн")]
        [RequireOwner()]
        public async Task balanceupd(IGuildUser user = null, string upd = null)
        {
            if (upd != null && user != null)
            {
                try
                {
                    if (int.Parse(upd) != 0)
                    {
                        myConnection.Open();
                        await new OleDbCommand("UPDATE Test SET d_money = " + (int.Parse(upd) + int.Parse(new OleDbCommand("SELECT d_money FROM Test WHERE d_dis = '" + user.Id + "'", myConnection).ExecuteScalar().ToString())).ToString() + " WHERE d_dis = '" + user.Id + "'", myConnection).ExecuteNonQueryAsync();
                        if (int.Parse(new OleDbCommand("SELECT d_money FROM Test WHERE d_dis = '" + user.Id + "'", myConnection).ExecuteScalar().ToString()) < 0)
                        {
                            await new OleDbCommand("UPDATE Test SET d_money = " + 0 + " WHERE d_dis = '" + user.Id + "'", myConnection).ExecuteNonQueryAsync();
                        }
                        await ReplyAsync("``Баланс пользователя:`` " + new OleDbCommand("SELECT d_money FROM Test WHERE d_dis = '" + user.Id + "'", myConnection).ExecuteScalar().ToString() + " ``лека``");
                        myConnection.Close();
                    }
                }
                catch
                {
                    await ReplyAsync("``Ошибка!``");
                }
            }
        }

        [Command("баланс")]
        public async Task balance()
        {
            myConnection.Open();
            string s = new OleDbCommand("SELECT d_money FROM Test WHERE d_dis = '" + Context.User.Id + "'", myConnection).ExecuteScalar().ToString();
            await ReplyAsync("``Ваш баланс:`` " + s + " ``лека``");
            myConnection.Close();
        }

        [Command("передать")]
        public async Task give(IGuildUser user = null, string sum = null)
        {
            try
            {
                if (sum == null || user == null || int.Parse(sum) <= 0)
                {
                    await ReplyAsync("``Ошибка!``");
                }
                else if (int.Parse(sum) < 50)
                {
                    await ReplyAsync("``Нельзя передавать меньше 50 лека!``");
                }
                else if (user == Context.User)
                {
                    await ReplyAsync("``Нельзя передавать лека себе!``");
                }
                else if (int.Parse(sum) < 50)
                {
                    await ReplyAsync("``Нельзя передавать меньше 50 лека!``");
                }
                else
                {
                    myConnection.Open();
                    string s = new OleDbCommand("SELECT d_money FROM Test WHERE d_dis = '" + Context.User.Id + "'", myConnection).ExecuteScalar().ToString();
                    if(int.Parse(s) < int.Parse(sum))
                    {
                        await ReplyAsync("``Недостаточно лека на балансе!``");
                    }
                    else
                    {
                        await ReplyAsync("``Комиссия составит 10% от суммы перевода``");
                        string s1 = new OleDbCommand("SELECT d_money FROM Test WHERE d_dis = '" + user.Id + "'", myConnection).ExecuteScalar().ToString();
                        int money = int.Parse(s1) + int.Parse(sum) - (int)Math.Round(double.Parse(sum) / 10d);
                        int money2 = int.Parse(s) - int.Parse(sum);
                        await new OleDbCommand("UPDATE Test SET d_money = " + money + " WHERE d_dis = '" + user.Id + "'", myConnection).ExecuteNonQueryAsync();
                        await new OleDbCommand("UPDATE Test SET d_money = " + money2 + " WHERE d_dis = '" + Context.User.Id + "'", myConnection).ExecuteNonQueryAsync();
                        await ReplyAsync("``Вы успешно перевели ``" + user.Mention + " " + (int.Parse(sum) - (int)Math.Round(double.Parse(sum) / 10d)).ToString() + "``лека``\n``Комиссия составила:`` " + ((int)Math.Round(double.Parse(sum) / 10d)).ToString() + " ``лека``\n``Ваш баланс: ``" + money2.ToString() + " ``лека``");
                    }
                    myConnection.Close();
                }
            }
            catch
            {
                await ReplyAsync("``Ошибка!``");
            }
        }

        [Command("лвл")]
        public async Task level()
        {
            myConnection.Open();
            string response = "";
            int exp = int.Parse(new OleDbCommand("SELECT d_exp FROM Test WHERE d_dis = '" + Context.User.Id + "'", myConnection).ExecuteScalar().ToString());
            response = "\n``Опыт:`` " + exp.ToString() + "\n``Ранг:`` ";
            if (exp < 100)
            {
                response += "1 ранг(Новичок)\n``Опыта до следующего ранга:`` " + (100 - exp).ToString();
            }
            else if (exp < 200)
            {
                response += "2 ранг(Д-класс)\n``Опыта до следующего ранга:`` " + (200 - exp).ToString();
            }
            else if (exp < 400)
            {
                response += "3 ранг(Научный сотрудник)\n``Опыта до следующего ранга:`` " + (400 - exp).ToString();
            }
            else if (exp < 800)
            {
                response += "4 ранг(Старший научный сотрудник)\n``Опыта до следующего ранга:`` " + (800 - exp).ToString();
            }
            else if (exp < 1600)
            {
                response += "5 ранг(Кадет МОГ)\n``Опыта до следующего ранга:`` " + (1600 - exp).ToString();
            }
            else if (exp < 3200)
            {
                response += "6 ранг(Лейтенант МОГ)\n``Опыта до следующего ранга:`` " + (3200 - exp).ToString();
            }
            else if (exp < 6400)
            {
                response += "7 ранг(Командир МОГ)\n``Опыта до следующего ранга:`` " + (6400 - exp).ToString();
            }
            else if (exp < 12800)
            {
                response += "8 ранг(ХАОС)\n``Опыта до следующего ранга:`` " + (12800 - exp).ToString();
            }
            else if (exp < 25600)
            {
                response += "9 ранг(Длань Змея)\n``Опыта до следующего ранга:`` " + (25600 - exp).ToString();
            }
            else if (exp < 51200)
            {
                response += "10 ранг(ГОК)\n``Опыта до следующего ранга:`` " + (51200 - exp).ToString();
            }
            else if (exp < 102400)
            {
                response += "11 ранг(Совет)\n``Опыта до следующего ранга:`` " + (102400 - exp).ToString();
            }
            else if (exp >= 102400)
            {
                response += "12 ранг(Председатель совета 05)";
            }
            await ReplyAsync(response);
            myConnection.Close();
        }

        [Command("балансигрок")]
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "``У вас нет полномочий!``")]
        public async Task balanceus(IGuildUser user = null)
        {
            if (user != null)
            {
                myConnection.Open();
                string s = new OleDbCommand("SELECT d_money FROM Test WHERE d_dis = '" + user.Id + "'", myConnection).ExecuteScalar().ToString();
                await ReplyAsync("``Его баланс:`` " + s + " ``лека``");
                myConnection.Close();
            }
        }

        [Command("варнигра")]
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "``У вас нет полномочий!``")]
        public async Task warngame(IGuildUser user = null, [Remainder] string reason = null)
        {
            try
            {
                if (user != null && Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(585767086815576069)) == true && user.IsBot == false)
                {
                    if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(586254953073475588)) == true)
                    {
                        await ReplyAsync("``Невозможно заварнить`` " + user.Mention + "``, так как у него уже 3/3 варнов``");
                    }
                    else
                    {
                        if (reason == null)
                        {
                            await ReplyAsync(user.Mention + " ``был успешно заварнен! Причина:`` не указана");
                        }
                        else
                        {
                            await ReplyAsync(user.Mention + " ``был успешно заварнен! Причина:`` " + reason);
                        }
                        if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(767089592574410753)) == true)
                        {
                            await user.RemoveRoleAsync(Context.Guild.GetRole(767089592574410753));
                        }
                        else
                        {
                            if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(656849681770479616)) == true)
                            {
                                if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(586254837943894016)) == true)
                                {
                                    await user.AddRoleAsync(Context.Guild.GetRole(586254913198096394));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(586254837943894016));
                                }
                                else if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(586254913198096394)) == true)
                                {
                                    await user.AddRoleAsync(Context.Guild.GetRole(585767086815576069));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(586254913198096394));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(656849681770479616));
                                }
                                else
                                {
                                    await user.AddRoleAsync(Context.Guild.GetRole(586254837943894016));
                                }
                            }
                            else
                            {
                                if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(586254837943894016)) == true)
                                {
                                    await user.AddRoleAsync(Context.Guild.GetRole(586254913198096394));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(586254837943894016));
                                }
                                else if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(586254913198096394)) == true)
                                {
                                    await user.AddRoleAsync(Context.Guild.GetRole(585767086815576069));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(586254913198096394));
                                }
                                else
                                {
                                    await user.AddRoleAsync(Context.Guild.GetRole(586254837943894016));
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                await ReplyAsync("``Ошибка!``");
            }
        }

        [Command("варндискорд")]
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "``У вас нет полномочий!``")]
        public async Task warndis(IGuildUser user = null, [Remainder] string reason = null)
        {
            try
            {
                if (user != null && Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(585767086815576069)) == true && user.IsBot == false)
                {
                    if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(586254953073475588)) == true)
                    {
                        await ReplyAsync("``Невозможно заварнить`` " + user.Mention + "``, так как у него уже 3/3 варнов``");
                    }
                    else
                    {
                        if(reason == null)
                        {
                            await ReplyAsync(user.Mention + " ``был успешно заварнен! Причина:`` не указана");
                        }
                        else
                        {
                            await ReplyAsync(user.Mention + " ``был успешно заварнен! Причина:`` " + reason);
                        }
                        if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(767089385585639454)) == true)
                        {
                            await user.RemoveRoleAsync(Context.Guild.GetRole(767089385585639454));
                        }
                        else
                        {
                            if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(656849681770479616)) == true)
                            {
                                if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(586254837943894016)) == true)
                                {
                                    await user.AddRoleAsync(Context.Guild.GetRole(586254913198096394));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(586254837943894016));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(656849681770479616));
                                }
                                else if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(586254913198096394)) == true)
                                {
                                    await user.AddRoleAsync(Context.Guild.GetRole(585767086815576069));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(586254913198096394));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(656849681770479616));
                                }
                                else
                                {
                                    await user.AddRoleAsync(Context.Guild.GetRole(586254837943894016));
                                    await user.RemoveRoleAsync(Context.Guild.GetRole(656849681770479616));
                                }
                            }
                            else
                            {
                                await user.AddRoleAsync(Context.Guild.GetRole(656849681770479616));
                            }
                        }
                    }
                }
            }
            catch
            {
                await ReplyAsync("``Ошибка!``");
            }
        }

        [Command("варндонат")]
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "``У вас нет полномочий!``")]
        public async Task warndonate(IGuildUser user = null, [Remainder] string reason = null)
        {
            try
            {
                if (user != null && Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(588694436456955904)) == true && user.IsBot == false)
                {
                    if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(803181956686151720)) == true)
                    {
                        await ReplyAsync("``Невозможно заварнить`` " + user.Mention + "``, так как у него уже 3/3 варнов``");
                    }
                    else
                    {
                        if (reason == null)
                        {
                            await ReplyAsync(user.Mention + " ``был успешно заварнен! Причина:`` не указана");
                        }
                        else
                        {
                            await ReplyAsync(user.Mention + " ``был успешно заварнен! Причина:`` " + reason);
                        }
                        if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(799325368620023809)) == true)
                        {
                            await user.AddRoleAsync(Context.Guild.GetRole(799325635209068584));
                            await user.RemoveRoleAsync(Context.Guild.GetRole(799325368620023809));
                        }
                        else if (Context.Guild.GetUser(user.Id).Roles.Contains(Context.Guild.GetRole(799325635209068584)) == true)
                        {
                            await user.AddRoleAsync(Context.Guild.GetRole(803181956686151720));
                            await user.RemoveRoleAsync(Context.Guild.GetRole(799325635209068584));
                        }
                        else
                        {
                            await user.AddRoleAsync(Context.Guild.GetRole(799325368620023809));
                        }
                    }
                }
            }
            catch
            {
                await ReplyAsync("``Ошибка!``");
            }
        }

        [Command("лвлигрок")]
        [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "``У вас нет полномочий!``")]
        public async Task levelus(IGuildUser user = null)
        {
            if (user != null)
            {
                myConnection.Open();
                string response = "";
                int exp = int.Parse(new OleDbCommand("SELECT d_exp FROM Test WHERE d_dis = '" + user.Id + "'", myConnection).ExecuteScalar().ToString());
                response = "\n``Опыт:`` " + exp.ToString() + "\n``Ранг:`` ";
                if (exp < 100)
                {
                    response += "1 ранг(Новичок)\n``Опыта до следующего ранга:`` " + (100 - exp).ToString();
                }
                else if (exp < 200)
                {
                    response += "2 ранг(Д-класс)\n``Опыта до следующего ранга:`` " + (200 - exp).ToString();
                }
                else if (exp < 400)
                {
                    response += "3 ранг(Научный сотрудник)\n``Опыта до следующего ранга:`` " + (400 - exp).ToString();
                }
                else if (exp < 800)
                {
                    response += "4 ранг(Старший научный сотрудник)\n``Опыта до следующего ранга:`` " + (800 - exp).ToString();
                }
                else if (exp < 1600)
                {
                    response += "5 ранг(Кадет МОГ)\n``Опыта до следующего ранга:`` " + (1600 - exp).ToString();
                }
                else if (exp < 3200)
                {
                    response += "6 ранг(Лейтенант МОГ)\n``Опыта до следующего ранга:`` " + (3200 - exp).ToString();
                }
                else if (exp < 6400)
                {
                    response += "7 ранг(Командир МОГ)\n``Опыта до следующего ранга:`` " + (6400 - exp).ToString();
                }
                else if (exp < 12800)
                {
                    response += "8 ранг(ХАОС)\n``Опыта до следующего ранга:`` " + (12800 - exp).ToString();
                }
                else if (exp < 25600)
                {
                    response += "9 ранг(Длань Змея)\n``Опыта до следующего ранга:`` " + (25600 - exp).ToString();
                }
                else if (exp < 51200)
                {
                    response += "10 ранг(ГОК)\n``Опыта до следующего ранга:`` " + (51200 - exp).ToString();
                }
                else if (exp < 102400)
                {
                    response += "11 ранг(Совет)\n``Опыта до следующего ранга:`` " + (102400 - exp).ToString();
                }
                else if (exp >= 102400)
                {
                    response += "12 ранг(Председатель совета 05)";
                }
                await ReplyAsync(response);
                myConnection.Close();
            }
        }

        [Command("обновить")]
        public async Task update(string steamid = null)
        {
            try
            {
                if (steamid == null || int.Parse(steamid) < 0)
                {
                    await ReplyAsync("``Введите новый steamid64!``");
                    return;
                }
                myConnection.Open();
                if (Context.Guild.GetUser(Context.User.Id).Roles.Contains(Context.Guild.GetRole(716606431578685502)) == true && await new OleDbCommand("SELECT d_dis FROM Test WHERE d_steam = '" + steamid + "'", myConnection).ExecuteScalarAsync() == null)
                {

                    await new OleDbCommand("UPDATE Test SET d_steam = '" + steamid + "' WHERE d_dis = '" + Context.User.Id + "'", myConnection).ExecuteNonQueryAsync();
                    await ReplyAsync("``Успешно``");
                }
                else
                {
                    await ReplyAsync("``Этот steamid уже зарегистрирован, или вы не верифицированы!``");
                }
                myConnection.Close();
            }
            catch
            {
                await ReplyAsync("``Ошибка!``");
            }
        }
    }
}
