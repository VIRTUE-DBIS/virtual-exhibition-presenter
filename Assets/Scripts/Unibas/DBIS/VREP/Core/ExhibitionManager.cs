using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.VREM.Model;
using Unibas.DBIS.VREP.World;
using UnityEngine;
using World;

namespace Unibas.DBIS.VREP.Core
{
    public class ExhibitionManager
    {
        private Exhibition _exhibition;

        public ExhibitionManager(Exhibition exhibition)
        {
            this._exhibition = exhibition;
        }

        private List<CuboidExhibitionRoom> _rooms = new List<CuboidExhibitionRoom>();

        public Exhibition GetExhibition()
        {
            return _exhibition;
        }
        
        public CuboidExhibitionRoom GetRoomByIndex(int index)
        {
            return _rooms[index];
        }

        public void RestoreExhibits() {
            _rooms.ForEach(r => r.RestoreWallExhibits());
        }

        private int GetNextRoomIndex(int pos)
        {
            return (pos + 1) % _exhibition.rooms.Length;
        }

        private int GetPreviousRoomIndex(int pos)
        {
            return (pos - 1 + _exhibition.rooms.Length) % _exhibition.rooms.Length;
        }

        private int GetRoomIndex(DefaultNamespace.VREM.Model.Room room)
        {
            for (int i = 0; i < _exhibition.rooms.Length; i++)
            {
                if (room.Equals(_exhibition.rooms[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private DefaultNamespace.VREM.Model.Room GetNext(DefaultNamespace.VREM.Model.Room room)
        {
            var pos = GetRoomIndex(room);
            if (pos == -1)
            {
                // TODO This should not happen
                return null;
            }

            return _exhibition.rooms[GetNextRoomIndex(pos)];
        }

        private DefaultNamespace.VREM.Model.Room GetPrevious(DefaultNamespace.VREM.Model.Room room)
        {
            var pos = GetRoomIndex(room);
            if (pos == -1)
            {
                // TODO This should not happen
                return null;
            }

            return _exhibition.rooms[GetPreviousRoomIndex(pos)];
        }

        public void GenerateExhibition()
        {
            foreach (var room in _exhibition.rooms)
            {
                var roomGameObject = ObjectFactory.BuildRoom(room);
                var exhibitionRoom = roomGameObject.GetComponent<CuboidExhibitionRoom>();
                _rooms.Add(exhibitionRoom);

                if (VREPController.Instance.Settings.CeilingLogoEnabled)
                {
                    GameObject pref = Resources.Load<GameObject>("Objects/unibas");
                    var logo = GameObject.Instantiate(pref);
                    logo.name = "UnibasLogo";
                    logo.transform.SetParent(exhibitionRoom.transform, false);
                    //logo.transform.localPosition = new Vector3(-1.493f, room.size.y-.01f, -0.642f); // manually found values
                    logo.transform.localPosition =
                        new Vector3(-1.493f, room.size.y - .01f, 3.35f); // manually found values
                    logo.transform.localRotation = Quaternion.Euler(new Vector3(90, 180));
                    logo.transform.localScale = Vector3.one * 10000;
                }
            }

            // For teleporting, each room needs to be created.
            foreach (var room in _rooms)
            {
                CreateAndAttachTeleporters(room);
            }
        }


        private void CreateAndAttachTeleporters(CuboidExhibitionRoom room)
        {
            var index = GetRoomIndex(room.RoomData);
            var next = _rooms[GetNextRoomIndex(index)];
            var prev = _rooms[GetPreviousRoomIndex(index)];

            var nd = next.GetEntryPoint();
            var pd = prev.GetEntryPoint();

            var backPos = new Vector3(-.25f, 0, .2f);
            var nextPos = new Vector3(.25f, 0, .2f);

            // TODO Configurable TPBtnModel
            var model = new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f,
                TexturingUtility.LoadMaterialByName("none"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"));

            if (_exhibition.rooms.Length > 1)
            {
                // back teleporter
                var backTpBtn = SteamVRTeleportButton.Create(room.gameObject, backPos, pd, model
                    ,
                    Resources.Load<Sprite>("Sprites/UI/chevron-left"));

                backTpBtn.OnTeleportStart = room.OnRoomLeave;
                backTpBtn.OnTeleportEnd = prev.OnRoomEnter;

                // back teleporter
                var nextTpBtn = SteamVRTeleportButton.Create(room.gameObject, nextPos, nd,
                    model,
                    Resources.Load<Sprite>("Sprites/UI/chevron-right"));

                nextTpBtn.OnTeleportStart = room.OnRoomLeave;
                nextTpBtn.OnTeleportEnd = next.OnRoomEnter;
            }


            if (VREPController.Instance.Settings.StartInLobby)
            {
                var lobbyTpBtn = SteamVRTeleportButton.Create(room.gameObject, new Vector3(0, 0, .2f),
                    VREPController.Instance.LobbySpawn,
                    model,
                    "Lobby");
                lobbyTpBtn.OnTeleportStart = room.OnRoomLeave;
            }
        }
    }
}