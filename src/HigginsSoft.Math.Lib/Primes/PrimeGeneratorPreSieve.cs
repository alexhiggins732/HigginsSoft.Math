﻿/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

#define LOG_OFFSETS
#undef LOG_OFFSETS

namespace HigginsSoft.Math.Lib
{
    public class PrimeGeneratorPreSieve
    {
        //11 - length:385
        public static uint[] Wheel11U => new uint[] { 0x96ED7B78, 0xCDEB361F, 0xBB56DF23, 0x7ADCA5E5, 0xC797CCBB, 0xB769F16E, 0xAD733EDC, 0xDA6E5BB5, 0x7CCFB3A5, 0x9F966D7A, 0x33EDEB56, 0xE4B95EDB, 0xDB7A9D27, 0x6AD7B7CC, 0x5EB669F9, 0xA5CDF33E, 0xADDA765B, 0x3B7CCEB5, 0x768F96ED, 0xDD33E9E3, 0xA7A53B5E, 0xC4FB6ADD, 0xF86CD7B3, 0x1EDEB769, 0x5BB5EDE3, 0x37ACD87E, 0xED5B7C8F, 0xEB729796, 0x1E5F32ED, 0xDDA7C5BB, 0xB5CCFB72, 0x69B96ED6, 0xF33ECE97, 0x7E59B5E9, 0xCFB7AD5A, 0x92E57B6C, 0x6DEA749F, 0xAB5EDF33, 0x7AD9A7E5, 0x9737CCF9, 0xB769D96E, 0xEDF33AD6, 0xDA3E5BB4, 0x74CFA78D, 0x9E94ED7B, 0x13EDAB76, 0xE1B35ECF, 0x7B7ADDA7, 0x6ED7B78C, 0xDEB361F9, 0xB56DF23C, 0xADCA5E5B, 0x797CCBB7, 0x769F16EC, 0xD733EDCB, 0xA6E5BB5A, 0xCCFB3A5D, 0xF966D7A7, 0x3EDEB569, 0x4B95EDB3, 0xB7A9D27E, 0xAD7B7CCD, 0xEB669F96, 0x5CDF33E5, 0xDDA765BA, 0xB7CCEB5A, 0x68F96ED3, 0xD33E9E37, 0x7A53B5ED, 0x4FB6ADDA, 0x86CD7B3C, 0xEDEB769F, 0xBB5EDE31, 0x7ACD87E5, 0xD5B7C8F3, 0xB729796E, 0xE5F32EDE, 0xDA7C5BB1, 0x5CCFB72D, 0x9B96ED6B, 0x33ECE976, 0xE59B5E9F, 0xFB7AD5A7, 0x2E57B6CC, 0xDEA749F9, 0xB5EDF336, 0xAD9A7E5A, 0x737CCF97, 0x769D96E9, 0xDF33AD6B, 0xA3E5BB4E, 0x4CFA78DD, 0xE94ED7B7, 0x3EDAB769, 0x1B35ECF1, 0xB7ADDA7E, 0xED7B78C7, 0xEB361F96, 0x56DF23CD, 0xDCA5E5BB, 0x97CCBB7A, 0x69F16EC7, 0x733EDCB7, 0x6E5BB5AD, 0xCFB3A5DA, 0x966D7A7C, 0xEDEB569F, 0xB95EDB33, 0x7A9D27E4, 0xD7B7CCDB, 0xB669F96A, 0xCDF33E5E, 0xDA765BA5, 0x7CCEB5AD, 0x8F96ED3B, 0x33E9E376, 0xA53B5EDD, 0xFB6ADDA7, 0x6CD7B3C4, 0xDEB769F8, 0xB5EDE31E, 0xACD87E5B, 0x5B7C8F37, 0x729796ED, 0x5F32EDEB, 0xA7C5BB1E, 0xCCFB72DD, 0xB96ED6B5, 0x3ECE9769, 0x59B5E9F3, 0xB7AD5A7E, 0xE57B6CCF, 0xEA749F92, 0x5EDF336D, 0xD9A7E5AB, 0x37CCF97A, 0x69D96E97, 0xF33AD6B7, 0x3E5BB4ED, 0xCFA78DDA, 0x94ED7B74, 0xEDAB769E, 0xB35ECF13, 0x7ADDA7E1, 0xD7B78C7B, 0xB361F96E, 0x6DF23CDE, 0xCA5E5BB5, 0x7CCBB7AD, 0x9F16EC79, 0x33EDCB76, 0xE5BB5AD7, 0xFB3A5DA6, 0x66D7A7CC, 0xDEB569F9, 0x95EDB33E, 0xA9D27E4B, 0x7B7CCDB7, 0x669F96AD, 0xDF33E5EB, 0xA765BA5C, 0xCCEB5ADD, 0xF96ED3B7, 0x3E9E3768, 0x53B5EDD3, 0xB6ADDA7A, 0xCD7B3C4F, 0xEB769F86, 0x5EDE31ED, 0xCD87E5BB, 0xB7C8F37A, 0x29796ED5, 0xF32EDEB7, 0x7C5BB1E5, 0xCFB72DDA, 0x96ED6B5C, 0xECE9769B, 0x9B5E9F33, 0x7AD5A7E5, 0x57B6CCFB, 0xA749F92E, 0xEDF336DE, 0x9A7E5AB5, 0x7CCF97AD, 0x9D96E973, 0x33AD6B76, 0xE5BB4EDF, 0xFA78DDA3, 0x4ED7B74C, 0xDAB769E9, 0x35ECF13E, 0xADDA7E1B, 0x7B78C7B7, 0x361F96ED, 0xDF23CDEB, 0xA5E5BB56, 0xCCBB7ADC, 0xF16EC797, 0x3EDCB769, 0x5BB5AD73, 0xB3A5DA6E, 0x6D7A7CCF, 0xEB569F96, 0x5EDB33ED, 0x9D27E4B9, 0xB7CCDB7A, 0x69F96AD7, 0xF33E5EB6, 0x765BA5CD, 0xCEB5ADDA, 0x96ED3B7C, 0xE9E3768F, 0x3B5EDD33, 0x6ADDA7A5, 0xD7B3C4FB, 0xB769F86C, 0xEDE31EDE, 0xD87E5BB5, 0x7C8F37AC, 0x9796ED5B, 0x32EDEB72, 0xC5BB1E5F, 0xFB72DDA7, 0x6ED6B5CC, 0xCE9769B9, 0xB5E9F33E, 0xAD5A7E59, 0x7B6CCFB7, 0x749F92E5, 0xDF336DEA, 0xA7E5AB5E, 0xCCF97AD9, 0xD96E9737, 0x3AD6B769, 0x5BB4EDF3, 0xA78DDA3E, 0xED7B74CF, 0xAB769E94, 0x5ECF13ED, 0xDDA7E1B3, 0xB78C7B7A, 0x61F96ED7, 0xF23CDEB3, 0x5E5BB56D, 0xCBB7ADCA, 0x16EC797C, 0xEDCB769F, 0xBB5AD733, 0x3A5DA6E5, 0xD7A7CCFB, 0xB569F966, 0xEDB33EDE, 0xD27E4B95, 0x7CCDB7A9, 0x9F96AD7B, 0x33E5EB66, 0x65BA5CDF, 0xEB5ADDA7, 0x6ED3B7CC, 0x9E3768F9, 0xB5EDD33E, 0xADDA7A53, 0x7B3C4FB6, 0x769F86CD, 0xDE31EDEB, 0x87E5BB5E, 0xC8F37ACD, 0x796ED5B7, 0x2EDEB729, 0x5BB1E5F3, 0xB72DDA7C, 0xED6B5CCF, 0xE9769B96, 0x5E9F33EC, 0xD5A7E59B, 0xB6CCFB7A, 0x49F92E57, 0xF336DEA7, 0x7E5AB5ED, 0xCF97AD9A, 0x96E9737C, 0xAD6B769D, 0xBB4EDF33, 0x78DDA3E5, 0xD7B74CFA, 0xB769E94E, 0xECF13EDA, 0xDA7E1B35, 0x78C7B7AD, 0x1F96ED7B, 0x23CDEB36, 0xE5BB56DF, 0xBB7ADCA5, 0x6EC797CC, 0xDCB769F1, 0xB5AD733E, 0xA5DA6E5B, 0x7A7CCFB3, 0x569F966D, 0xDB33EDEB, 0x27E4B95E, 0xCCDB7A9D, 0xF96AD7B7, 0x3E5EB669, 0x5BA5CDF3, 0xB5ADDA76, 0xED3B7CCE, 0xE3768F96, 0x5EDD33E9, 0xDDA7A53B, 0xB3C4FB6A, 0x69F86CD7, 0xE31EDEB7, 0x7E5BB5ED, 0x8F37ACD8, 0x96ED5B7C, 0xEDEB7297, 0xBB1E5F32, 0x72DDA7C5, 0xD6B5CCFB, 0x9769B96E, 0xE9F33ECE, 0x5A7E59B5, 0x6CCFB7AD, 0x9F92E57B, 0x336DEA74, 0xE5AB5EDF, 0xF97AD9A7, 0x6E9737CC, 0xD6B769D9, 0xB4EDF33A, 0x8DDA3E5B, 0x7B74CFA7, 0x769E94ED, 0xCF13EDAB, 0xA7E1B35E, 0x8C7B7ADD, 0xF96ED7B7, 0x3CDEB361, 0x5BB56DF2, 0xB7ADCA5E, 0xEC797CCB, 0xCB769F16, 0x5AD733ED, 0x5DA6E5BB, 0xA7CCFB3A, 0x69F966D7, 0xB33EDEB5, 0x7E4B95ED, 0xCDB7A9D2, 0x96AD7B7C, 0xE5EB669F, 0xBA5CDF33, 0x5ADDA765, 0xD3B7CCEB, 0x3768F96E, 0xEDD33E9E, 0xDA7A53B5, 0x3C4FB6AD, 0x9F86CD7B, 0x31EDEB76, 0xE5BB5EDE, 0xF37ACD87, 0x6ED5B7C8, 0xDEB72979, 0xB1E5F32E, 0x2DDA7C5B, 0x6B5CCFB7, 0x769B96ED, 0x9F33ECE9, 0xA7E59B5E, 0xCCFB7AD5, 0xF92E57B6, 0x36DEA749, 0x5AB5EDF3, 0x97AD9A7E, 0xE9737CCF, 0x6B769D96, 0x4EDF33AD, 0xDDA3E5BB, 0xB74CFA78, 0x69E94ED7, 0xF13EDAB7, 0x7E1B35EC, 0xC7B7ADDA };
        public static int[] Wheel11 => Wheel11U.Select(x => { unchecked { return (int)x; } })
                        .Concat(Wheel11U.Select(x => { unchecked { return (int)x; } })).ToArray();

        public static int[] expectedLow = new int[] { 49305, 49299, 49303, 49321, 49329, 49319, 49303, 49353, 49287, 49285, 49291, 49391, 49323, 49397, 49379, 49361, 49353, 49317, 49327, 49299, 49471, 49381, 49491, 49497, 49431, 49309, 49425, 49353, 49381, 49495, 49469, 49385, 49497, 49345, 49469, 49591, 49557, 49591, 49341, 49517, 49561, 49631, 49585, 49367, 49449, 49549, 49539, 49493, 49445, 49385, 49683, 49299, 49495, 49449, 49713, 49825, 49599, 49515, 49371, 49905, 49689, 49837, 49831, 49839, 49531, 49581, 49945, 49829, 50017, 49375, 49915, 49907, 50049, 49675, 49633, 49625, 49771, 49559, 49669, 49967, 50139, 49459, 49659, 49727, 49753, 49789, 49769, 49649, 49557, 49309, 50159, 49897, 50271, 50145, 49853, 49311, 49415, 49883, 50327, 50089, 49343, 50341, 49491, 49791, 49485, 49749, 49989, 49313, 50205, 49303, 49937, 49543, 50033, 50105, 50153, 49723, 52415, 54215, 56935, 60599, 65239, 68999, 73759, 77615, 80535, 83479, 85455, 89439, 92455, 94479, 98559, 100615, 107895, 113175, 119599, 120679, 126119, 127215, 129415, 130519, 136079, 143975, 146255, 147399, 149695, 157815, 160159, 161335, 163695, 175655, 178079, 182959, 189119, 194095, 196599, 200375, 204175, 213135, 215719, 219615, 223535, 228799, 232775, 240799, 243495, 247559 };

        public static int[] expectedHigh = new int[] { 49296, 49310, 49290, 49290, 49290, 49298, 49352, 49298, 49344, 49316, 49326, 49312, 49282, 49352, 49284, 49312, 49300, 49372, 49386, 49428, 49336, 49312, 49348, 49424, 49280, 49478, 49512, 49444, 49288, 49296, 49368, 49280, 49388, 49456, 49584, 49352, 49436, 49336, 49598, 49648, 49428, 49490, 49436, 49518, 49296, 49704, 49698, 49332, 49612, 49556, 49332, 49478, 49314, 49818, 49338, 49636, 49794, 49310, 49578, 49696, 49900, 49616, 49606, 49376, 49298, 49816, 49466, 49584, 49768, 49880, 49404, 49388, 49784, 49942, 49360, 49904, 49490, 49846, 49380, 49674, 49548, 49758, 49354, 50034, 49444, 50100, 50088, 49324, 49884, 49974, 49488, 50236, 49576, 49796, 49492, 50040, 49786, 50258, 49568, 49708, 50112, 49558, 49886, 50190, 50286, 49344, 49580, 49724, 49792, 50144, 50364, 50400, 50464, 50540, 50592, 49282, 53312, 54666, 57390, 61520, 65706, 69944, 74238, 78584, 81512, 84464, 85950, 90440, 93464, 94986, 99584, 101130, 108944, 113706, 120138, 121760, 126666, 128312, 129966, 131624, 136638, 145112, 146826, 148544, 150270, 158984, 160746, 162512, 164286, 176864, 178686, 184184, 189738, 195344, 197226, 201006, 204810, 214424, 216366, 220266, 224190, 230120, 234104, 242144, 244170, 248238 };
    }
}