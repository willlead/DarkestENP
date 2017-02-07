using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class UserPublishInfo
{
    public Guid m_guid;
    public String m_name;
    public int g_iUserLevel;
    public int g_iUserScore;

    public UserPublishInfo()
    {
        m_name = "none";
        g_iUserLevel = 0;
        g_iUserScore = 0;
    }
}

public class GameRoomParameter
{
    public Guid m_guid;
    public String m_name;
    public int m_type;
    public Guid m_masterHerGuid;
}

public class GameShopParameter
{
    public Guid m_guid;
    public String m_name;
    public int m_type;
    public int m_price;
}

public class GameOption
{
    public bool m_bgMusic = true;
    public bool m_effectSound = true;
    public bool m_MessageNoti = true;
}

enum GameFinishState
{
    NEXT_GAME_PLAY = 0, USER_LEVEL_UP, USER_ROOM_MAX_LEVEL, GAME_REPLAY 
}