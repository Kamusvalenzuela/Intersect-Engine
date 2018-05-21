﻿using System;
using Intersect.Logging;
using Intersect.Migration.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib;
using Intersect.Migration.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect.Migration.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using Mono.Data.Sqlite;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_2
{
    public class Upgrade2
    {
        //Ban Table Constants
        private const string BAN_TABLE = "bans";

        private const string BAN_ID = "id";
        private const string BAN_TIME = "time";
        private const string BAN_USER = "user";
        private const string BAN_IP = "ip";
        private const string BAN_DURATION = "duration";
        private const string BAN_REASON = "reason";
        private const string BAN_BANNER = "banner";

        //Mute Table Constants
        private const string MUTE_TABLE = "mutes";

        private const string MUTE_ID = "id";
        private const string MUTE_TIME = "time";
        private const string MUTE_USER = "user";
        private const string MUTE_IP = "ip";
        private const string MUTE_DURATION = "duration";
        private const string MUTE_REASON = "reason";
        private const string MUTE_MUTER = "muter";

        //GameObject Table Constants
        private const string GAME_OBJECT_ID = "id";

        private const string GAME_OBJECT_DELETED = "deleted";
        private const string GAME_OBJECT_DATA = "data";

        //Map List Table Constants
        private const string MAP_LIST_TABLE = "map_list";

        private const string MAP_LIST_DATA = "data";
        private SqliteConnection mDbConnection;
        private object mDbLock = new object();

        public Upgrade2(SqliteConnection connection)
        {
            mDbConnection = connection;
        }

        public void Upgrade()
        {
            //Upgrade Steps
            ServerOptions.LoadOptions();
            FixBansTable();
            FixMutesTable();
            ConvertNpcs();
            ConvertProjectiles();
        }

        //This adds a spell list to npcs that we can use later.
        public void ConvertNpcs()
        {
            LoadGameObjects(GameObject.Npc);
            foreach (var obj in NpcBase.GetObjects())
            {
                SaveGameObject(obj.Value);
            }
        }

        public void ConvertProjectiles()
        {
            LoadGameObjects(GameObject.Projectile);
            foreach (var obj in ProjectileBase.GetObjects())
            {
                SaveGameObject(obj.Value);
            }
        }

        //Game Object Saving/Loading
        private string GetGameObjectTable(GameObject type)
        {
            var tableName = "";
            switch (type)
            {
                case GameObject.Animation:
                    tableName = AnimationBase.DATABASE_TABLE;
                    break;
                case GameObject.Class:
                    tableName = ClassBase.DATABASE_TABLE;
                    break;
                case GameObject.Item:
                    tableName = ItemBase.DATABASE_TABLE;
                    break;
                case GameObject.Npc:
                    tableName = NpcBase.DATABASE_TABLE;
                    break;
                case GameObject.Projectile:
                    tableName = ProjectileBase.DATABASE_TABLE;
                    break;
                case GameObject.Quest:
                    tableName = QuestBase.DATABASE_TABLE;
                    break;
                case GameObject.Resource:
                    tableName = ResourceBase.DATABASE_TABLE;
                    break;
                case GameObject.Shop:
                    tableName = ShopBase.DATABASE_TABLE;
                    break;
                case GameObject.Spell:
                    tableName = SpellBase.DATABASE_TABLE;
                    break;
                case GameObject.Map:
                    tableName = MapBase.DATABASE_TABLE;
                    break;
                case GameObject.CommonEvent:
                    tableName = EventBase.DATABASE_TABLE;
                    break;
                case GameObject.PlayerSwitch:
                    tableName = PlayerSwitchBase.DATABASE_TABLE;
                    break;
                case GameObject.PlayerVariable:
                    tableName = PlayerVariableBase.DATABASE_TABLE;
                    break;
                case GameObject.ServerSwitch:
                    tableName = ServerSwitchBase.DATABASE_TABLE;
                    break;
                case GameObject.ServerVariable:
                    tableName = ServerVariableBase.DATABASE_TABLE;
                    break;
                case GameObject.Tileset:
                    tableName = TilesetBase.DATABASE_TABLE;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return tableName;
        }

        private void ClearGameObjects(GameObject type)
        {
            switch (type)
            {
                case GameObject.Animation:
                    AnimationBase.ClearObjects();
                    break;
                case GameObject.Class:
                    ClassBase.ClearObjects();
                    break;
                case GameObject.Item:
                    ItemBase.ClearObjects();
                    break;
                case GameObject.Npc:
                    NpcBase.ClearObjects();
                    break;
                case GameObject.Projectile:
                    ProjectileBase.ClearObjects();
                    break;
                case GameObject.Quest:
                    QuestBase.ClearObjects();
                    break;
                case GameObject.Resource:
                    ResourceBase.ClearObjects();
                    break;
                case GameObject.Shop:
                    ShopBase.ClearObjects();
                    break;
                case GameObject.Spell:
                    SpellBase.ClearObjects();
                    break;
                case GameObject.Map:
                    MapBase.ClearObjects();
                    break;
                case GameObject.CommonEvent:
                    EventBase.ClearObjects();
                    break;
                case GameObject.PlayerSwitch:
                    PlayerSwitchBase.ClearObjects();
                    break;
                case GameObject.PlayerVariable:
                    PlayerVariableBase.ClearObjects();
                    break;
                case GameObject.ServerSwitch:
                    ServerSwitchBase.ClearObjects();
                    break;
                case GameObject.ServerVariable:
                    ServerVariableBase.ClearObjects();
                    break;
                case GameObject.Tileset:
                    TilesetBase.ClearObjects();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void LoadGameObject(GameObject type, int index, byte[] data)
        {
            switch (type)
            {
                case GameObject.Animation:
                    var anim = new AnimationBase(index);
                    anim.Load(data);
                    AnimationBase.AddObject(index, anim);
                    break;
                case GameObject.Class:
                    var cls = new ClassBase(index);
                    cls.Load(data);
                    ClassBase.AddObject(index, cls);
                    break;
                case GameObject.Item:
                    var itm = new ItemBase(index);
                    itm.Load(data);
                    ItemBase.AddObject(index, itm);
                    break;
                case GameObject.Npc:
                    var npc = new NpcBase(index);
                    npc.Load(data);
                    NpcBase.AddObject(index, npc);
                    break;
                case GameObject.Projectile:
                    var proj = new ProjectileBase(index);
                    proj.Load(data);
                    ProjectileBase.AddObject(index, proj);
                    break;
                case GameObject.Quest:
                    var qst = new QuestBase(index);
                    qst.Load(data);
                    QuestBase.AddObject(index, qst);
                    break;
                case GameObject.Resource:
                    var res = new ResourceBase(index);
                    res.Load(data);
                    ResourceBase.AddObject(index, res);
                    break;
                case GameObject.Shop:
                    var shp = new ShopBase(index);
                    shp.Load(data);
                    ShopBase.AddObject(index, shp);
                    break;
                case GameObject.Spell:
                    var spl = new SpellBase(index);
                    spl.Load(data);
                    SpellBase.AddObject(index, spl);
                    break;
                case GameObject.Map:
                    var map = new MapBase(index, false);
                    MapBase.AddObject(index, map);
                    map.Load(data);
                    break;
                case GameObject.CommonEvent:
                    var buffer = new ByteBuffer();
                    buffer.WriteBytes(data);
                    var evt = new EventBase(index, buffer, true);
                    EventBase.AddObject(index, evt);
                    buffer.Dispose();
                    break;
                case GameObject.PlayerSwitch:
                    var pswitch = new PlayerSwitchBase(index);
                    pswitch.Load(data);
                    PlayerSwitchBase.AddObject(index, pswitch);
                    break;
                case GameObject.PlayerVariable:
                    var pvar = new PlayerVariableBase(index);
                    pvar.Load(data);
                    PlayerVariableBase.AddObject(index, pvar);
                    break;
                case GameObject.ServerSwitch:
                    var sswitch = new ServerSwitchBase(index);
                    sswitch.Load(data);
                    ServerSwitchBase.AddObject(index, sswitch);
                    break;
                case GameObject.ServerVariable:
                    var svar = new ServerVariableBase(index);
                    svar.Load(data);
                    ServerVariableBase.AddObject(index, svar);
                    break;
                case GameObject.Tileset:
                    var tset = new TilesetBase(index);
                    tset.Load(data);
                    TilesetBase.AddObject(index, tset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void LoadGameObjects(GameObject type)
        {
            var nullIssues = "";
            lock (mDbLock)
            {
                var tableName = GetGameObjectTable(type);
                ClearGameObjects(type);
                var query = "SELECT * from " + tableName + " WHERE " + GAME_OBJECT_DELETED + "=@" +
                            GAME_OBJECT_DELETED +
                            ";";
                using (SqliteCommand cmd = new SqliteCommand(query, mDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var index = Convert.ToInt32(dataReader[GAME_OBJECT_ID]);
                            if (dataReader[MAP_LIST_DATA].GetType() != typeof(DBNull))
                            {
                                LoadGameObject(type, index, (byte[]) dataReader[GAME_OBJECT_DATA]);
                            }
                            else
                            {
                                nullIssues += "Tried to load null value for index " + index + " of " + tableName +
                                              Environment.NewLine;
                            }
                        }
                    }
                }
            }
            if (nullIssues != "")
            {
                throw (new Exception("Tried to load one or more null game objects!" + Environment.NewLine +
                                     nullIssues));
            }
        }

        public void SaveGameObject(DatabaseObject gameObject)
        {
            if (gameObject == null)
            {
                Log.Error("Attempted to persist null game object to the database.");
            }

            lock (mDbLock)
            {
                var insertQuery = "UPDATE " + gameObject.GetTable() + " set " + GAME_OBJECT_DELETED + "=@" +
                                  GAME_OBJECT_DELETED + "," + GAME_OBJECT_DATA + "=@" + GAME_OBJECT_DATA + " WHERE " +
                                  GAME_OBJECT_ID + "=@" + GAME_OBJECT_ID + ";";
                using (SqliteCommand cmd = new SqliteCommand(insertQuery, mDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.GetId()));
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    if (gameObject.GetData() != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, gameObject.GetData()));
                    }
                    else
                    {
                        throw (new Exception("Tried to save a null game object (should be deleted instead?) Table: " +
                                             gameObject.GetTable() + " Id: " + gameObject.GetId()));
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void FixMutesTable()
        {
            var cmd = "DROP TABLE " + MUTE_TABLE;
            using (var createCommand = mDbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
            cmd = "CREATE TABLE " + MUTE_TABLE + " ("
                  + MUTE_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                  + MUTE_TIME + " TEXT,"
                  + MUTE_USER + " INTEGER,"
                  + MUTE_IP + " TEXT,"
                  + MUTE_DURATION + " INTEGER,"
                  + MUTE_REASON + " TEXT,"
                  + MUTE_MUTER + " TEXT"
                  + ");";
            using (var createCommand = mDbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        private void FixBansTable()
        {
            var cmd = "DROP TABLE " + BAN_TABLE;
            using (var createCommand = mDbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
            cmd = "CREATE TABLE " + BAN_TABLE + " ("
                  + BAN_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                  + BAN_TIME + " TEXT,"
                  + BAN_USER + " INTEGER,"
                  + BAN_IP + " TEXT,"
                  + BAN_DURATION + " INTEGER,"
                  + BAN_REASON + " TEXT,"
                  + BAN_BANNER + " TEXT"
                  + ");";
            using (var createCommand = mDbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }
    }
}