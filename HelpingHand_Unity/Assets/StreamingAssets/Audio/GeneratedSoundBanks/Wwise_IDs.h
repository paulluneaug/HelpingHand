/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID MAINMUSIC_PLAY = 795638160U;
        static const AkUniqueID MAINMUSIC_STOP = 3439190578U;
        static const AkUniqueID ONPOINTERDOWN = 1065012045U;
        static const AkUniqueID ONPOINTERENTER = 414303029U;
        static const AkUniqueID ONPOINTEREXIT = 3466097449U;
        static const AkUniqueID ONPOINTERUP = 3836048698U;
        static const AkUniqueID PLAY_FADERFADEOUT = 3243193900U;
        static const AkUniqueID PLAY_FADERIMMEDIATE = 2824786805U;
        static const AkUniqueID PLAY_FADERLOOP = 3688189956U;
        static const AkUniqueID PLAY_FADERMAX = 3224940184U;
        static const AkUniqueID PLAY_FADERMIN = 3359161110U;
        static const AkUniqueID PLAY_FOOTSTEPS = 3854155799U;
        static const AkUniqueID PLAY_ROOMMACHINIST = 2948635231U;
        static const AkUniqueID PLAY_SQUAREROCKLOOP = 2367626798U;
        static const AkUniqueID PLAY_SQUAREROCKMAX = 3543323798U;
        static const AkUniqueID PLAY_SQUAREROCKMIN = 3409102904U;
        static const AkUniqueID PLAY_THEATERAMBIENCE = 3213662481U;
        static const AkUniqueID STOP_FOOTSTEPS = 2963349357U;
        static const AkUniqueID STOP_SQUAREROCKFADEOUT = 1184221932U;
        static const AkUniqueID STOP_SQUAREROCKIMMEDIATE = 2999759669U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GAMESTATE
        {
            static const AkUniqueID GROUP = 4091656514U;

            namespace STATE
            {
                static const AkUniqueID GAMEOVER = 4158285989U;
                static const AkUniqueID GAMEPLAY = 89505537U;
                static const AkUniqueID MAINMENU = 3604647259U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID PAUSED = 319258907U;
            } // namespace STATE
        } // namespace GAMESTATE

        namespace MUSICSTATE
        {
            static const AkUniqueID GROUP = 1021618141U;

            namespace STATE
            {
                static const AkUniqueID GAMEPLAY1STSECTION = 1738956672U;
                static const AkUniqueID GAMEPLAY2NDSECTION = 311720168U;
                static const AkUniqueID GAMEPLAY3RDSECTION = 490107267U;
                static const AkUniqueID LEVEL_LOSE = 1003524675U;
                static const AkUniqueID LEVEL_START = 352576276U;
                static const AkUniqueID LEVEL_WIN = 2908556826U;
                static const AkUniqueID MAINMENU = 3604647259U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID PAUSEMENU = 3494343696U;
            } // namespace STATE
        } // namespace MUSICSTATE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace LOCOMOTION_TYPE
        {
            static const AkUniqueID GROUP = 748991833U;

            namespace SWITCH
            {
                static const AkUniqueID IDLE = 1874288895U;
                static const AkUniqueID RUN = 712161704U;
                static const AkUniqueID STAIRS = 1289942167U;
                static const AkUniqueID WALK = 2108779966U;
            } // namespace SWITCH
        } // namespace LOCOMOTION_TYPE

        namespace MATERIAL
        {
            static const AkUniqueID GROUP = 3865314626U;

            namespace SWITCH
            {
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID GRAVEL = 2185786256U;
                static const AkUniqueID ROCK = 2144363834U;
                static const AkUniqueID WOOD = 2058049674U;
            } // namespace SWITCH
        } // namespace MATERIAL

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID RTPC_MUSIC_FIRSTLAYER = 2116630962U;
        static const AkUniqueID RTPC_MUSIC_SECONDLAYER = 1485517750U;
        static const AkUniqueID RTPC_RANDOMSEEKPOSITION = 2865193319U;
        static const AkUniqueID SLIDERSPEED = 1460444787U;
        static const AkUniqueID SLIDERVALUE = 2333331917U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
        static const AkUniqueID MUSICS = 1730564753U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMBBUS = 2894416467U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MOVABLEOBJECTSBUS = 2112977367U;
        static const AkUniqueID MUSICBUS = 2886307548U;
        static const AkUniqueID PUPPETBUS = 1406467327U;
        static const AkUniqueID RVB = 695384145U;
        static const AkUniqueID SFXBUS = 3803850708U;
        static const AkUniqueID UIBUS = 1372881427U;
        static const AkUniqueID VOICEBUS = 2045367873U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID ENORMOUSRVB = 92103823U;
        static const AkUniqueID HALL = 3633416828U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
