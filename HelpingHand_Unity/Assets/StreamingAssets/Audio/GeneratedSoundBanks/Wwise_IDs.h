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
        static const AkUniqueID BLENDTRACK = 4198610215U;
        static const AkUniqueID MAINMUSIC_PLAY = 795638160U;
        static const AkUniqueID MAINMUSIC_STOP = 3439190578U;
        static const AkUniqueID ONPOINTERDOWN = 1065012045U;
        static const AkUniqueID ONPOINTERENTER = 414303029U;
        static const AkUniqueID ONPOINTEREXIT = 3466097449U;
        static const AkUniqueID ONPOINTERUP = 3836048698U;
        static const AkUniqueID PLAY_TEST_APPLAUSE_LOOP = 131427293U;
        static const AkUniqueID PLAY_TEST_BEEP = 27511013U;
        static const AkUniqueID PLAYFADERSFX = 3386389010U;
        static const AkUniqueID SLIDERMAXROCK = 2299486555U;
        static const AkUniqueID SLIDERMINROCK = 1132753153U;
        static const AkUniqueID STOPFADERSFX_FADEOUT = 531241159U;
        static const AkUniqueID STOPFADERSFX_IMMEDIATE = 217122686U;
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
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSICBUS = 2886307548U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
