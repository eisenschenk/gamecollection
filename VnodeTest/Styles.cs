using ACL.CSS;
using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;

namespace VnodeTest
{
    using static ACL.CSS.DOM;

    public class Styles : ACL.UI.React.DOM.DefaultStyles
    {
        static Styles() => RegisterStyles<Styles>();

        public Rule[] GetRules() => GetRulesOfType(this);

        public Rule[] SomeRules => RuleSet(
                          Def(BCblack,
                BackgroundColor("black")
            ),

            Def(TCblack,
                Color("black")
            ),

            Def(TCred,
                Color("red")
            ),

            Def(TCwhite,
                Color("white")
            ),

            Def(TCgreen,
                Color("green")
            ),

            Def(Dropdown
                , Display("block")
                , Position("absolute")
                , BackgroundColor("lightgray")
                , Width("fit-content")
                , Height("auto")
                , Padding("12px 16px")
                , ZIndex("1")
            ),

            Def(MP2,
                Margin("2px")
            ),

            Def(Fontcopperplate,
                FontFamily("Copperplate"),
                FontSize("24px")
            ),

             Def(FontBrushscriptmt,
                FontFamily("Brush Script MT"),
                FontSize("24px")
            ),

              Def(MR0P5,
                MarginRight("0.5rem")
            ),

            Def(M2,
                Margin("2rem")
            ),

            Def(MP4,
                Margin("4px")
            ),

            Def(MP8,
                Margin("8px")
            ),

            Def(Ml6,
                Margin("0rem 0rem 0rem 5rem")
            ),

            Def(MX2,
                MarginLeft("2px"),
                MarginRight("2px")
            ),

            Def(MX1,
                MarginLeft("1rem"),
                MarginRight("1rem")
            ),

            Def(MY1,
                MarginTop("1rem"),
                MarginBottom("1rem")
            ),

            Def(MY2,
                MarginTop("2rem"),
                MarginBottom("2rem")
            ),

             Def(MY6,
                MarginTop("6rem"),
                MarginBottom("6rem")
            ),

            Def(MR2,
                MarginRight("2px")
            ),

            Def(ML2,
                MarginLeft("2px")
            ),

             Def(ML8,
                MarginLeft("8rem")
            ),

            Def(MT1,
                MarginTop("1rem")
            ),


            Def(MT2,
                MarginTop("2px")
            ),

            Def(MB2,
                MarginBottom("2px")
            ),


            Def(R10,
                Right("10px")
            ),

            Def(Absolute,
                Position("absolute")
            ),

            Def(TabNameTag,
                MarginTop("1rem"),
                Padding("4px"),
                Margin("0.25rem"),
                Width("7rem"),
                BackgroundColor("mediumslateblue"),
                Color("linen"),
                Border("solid"),
                BorderColor("mediumslateblue"),
                TextAlignC
            ),

            Def(TabNameTagNoWidth,
                MarginTop("1rem"),
                Padding("4px"),
                Margin("0.25rem"),
                BackgroundColor("mediumslateblue"),
                Color("linen"),
                Border("solid"),
                BorderColor("mediumslateblue")
            ),

            Def(TabNameTagNoWidthMargin,
                BackgroundColor("mediumslateblue"),
                Color("linen")
            ),

            Def(P4,
                Padding("4")
            ),

            Def(TabButtonSelected,
                MarginTop("1rem"),
                Padding("4px"),
                Margin("0.25rem"),
                Width("7rem"),
                BackgroundColor("brown"),
                Color("linen"),
                Border("solid"),
                BorderColor("linen"),
                TextAlignC
            ),

            Def(TabButton,
                MarginTop("1rem"),
                Padding("4px"),
                Margin("0.25rem"),
                Width("7rem"),
                BackgroundColor("yellowgreen"),
                Color("linen"),
                Border("solid"),
                BorderColor("linen"),
                TextAlignC
            ),

             Def(BtnSettings,
                PaddingRight("3px"),
                PaddingLeft("3px"),
                PaddingBottom("3px"),
                BackgroundColor("yellowgreen"),
                Color("linen"),
                TextAlignC
            ),

            Def(TabButtonSmall,
                MarginTop("1rem"),
                Padding("4px"),
                Margin("0.25rem"),
                Width("4rem"),
                BackgroundColor("yellowgreen"),
                Color("linen"),
                Border("solid"),
                BorderColor("linen"),
                TextAlignC
            ),

             Def(TabMenuItem,
                MarginTop("1rem"),
                Padding("4px"),
                Margin("0.25rem"),
                Width("7rem"),
                BackgroundColor("mediumslateblue"),
                Color("linen"),
                Border("solid"),
                BorderColor("linen"),
                TextAlignC
            ),

             Def(TabMenuItemSelected,
                MarginTop("1rem"),
                Padding("4px"),
                Margin("0.25rem"),
                Width("7rem"),
                BackgroundColor("lightsalmon"),
                Color("linen"),
                Border("solid"),
                BorderColor("linen"),
                TextAlignC
            ),

              Def(MB2P5rem,
                MarginBottom("2.5rem")
            ),

            Def(MainMenuItem,
                MarginTop("1rem"),
                Padding("4px"),
                BackgroundColor("darkslategray"),
                Color("linen")
            ),

            Def(SubMenuItem,
                Padding("4px"),
                BackgroundColor("gray"),
                GetHoverRule("dimgray"),
                Color("Linen")
            ),

            Def(SubMenuItemSelected,
                Padding("4px"),
                BackgroundColor("dimgray"),
                Color("Linen")
            ),

            Def(Btn,
                BaseBtn,
                BackgroundColor("green"),
                GetHoverRule("darkgreen")
            ),

            Def(AbortBtn,
                BaseBtn,
                BackgroundColor("red"),
                GetHoverRule("orange")
            ),

            Def(SelectedBtn,
                BaseBtn,
                BackgroundColor("orange"),
                GetHoverRule("orange")
            ),

            Def(BaseBtn,
                BorderRadius("5px"),
                Width("fit-content"),
                Padding("4px")
            ),

            Def(Selected,
                Border("2px dashed purple")
            ),

            Def(BorderBlack,
                Border("2px solid slategray")
            ),

             Def(BorderWhite,
                Border("2px solid khaki")
            ),

            Def(TileBase
                , Width("4rem")
                , Height("4rem")
            ),

            Def(TileBlack
                , TileBase
                , BackgroundColor("slategray")
            ),

             Def(TileWhite
                , TileBase
                , BackgroundColor("khaki")
            ),

            Def(BorderedBox,
                Padding("2px"),
                BorderRadius(".25rem"),
                Width("fit-content"),
                BackgroundColor("wheat")
            ),

            Def(BorderedBoxPartialBase,
                Padding("2px"),
                BorderRadius(".25rem .25rem 0rem 0rem"),
                Width("fit-content"),
                BorderWidth("2px 2px 0px 2px"),
                BorderColor("black"),
                BackgroundColor("wheat")
            ),

            Def(BorderedBoxPartial,
                BorderedBoxPartialBase,
                BorderStyle("solid")
            ),

            Def(BorderedBoxPartialSelected,
                BorderedBoxPartialBase,
                BorderStyle("dashed")
            ),

            Def(BorderedBoxBlack,
                BorderedBox,
                Border("2px solid black")
            ),

            Def(BorderedBoxPurple,
                BorderedBox,
                Border("2px dashed purple")
            ),

            Def(Score,
                BorderedBox,
                Margin("0rem 2rem 2rem 2rem"),
                Border("2px dashed purple")
            ),

            Def(
                BorderedBoxGreen,
                    BorderedBox,
                    Border("2px solid green")
            ),

            Def(WinBox,
                BorderedBox,
                BorderStyle("double"),
                BorderWidth("4px"),
                BorderColor("green")
            ),

            Def(BorderedBoxRed,
                BorderedBox,
                Border("2px solid red")
            ),

            Def(BorderW2,
                BorderWidth("2px")
            ),

            Def(BorderW4,
                BorderWidth("4px")
            ),

            Def(CardBackPartial,
                Padding("2px"),
                BorderRadius(".25rem .25rem 0rem 0rem"),
                Width("fit-content"),
                BorderWidth("2px 2px 0px 2px"),
                BorderStyle("solid"),
                BorderColor("slategray"),
                BackgroundColor("wheat")
            ),

            Def(CardEmptyBorderGreen,
                BorderedBoxGreen,
                W4C,
                H6,
                MP2
            ),

            Def(CardBlack,
                BorderedBoxBlack,
                TCblack
            ),

            Def(CardRed,
                BorderedBoxRed,
                TCred
            ),

            Def(CardGreen,
                BorderedBoxGreen,
                TCgreen
            ),

            Def(TextAlignC,
                TextAlign("center")
            ),

             Def(TextAlignR,
                TextAlign("right")
            ),



            Def(Relative,
                Position("relative")
            ),

            Def(AlignItemRight,
                Position("absolute"),
                Right("0px"),
                Width("fit-content")
            ),

            Def(AlignItemCenter,
                Position("absolute"),
                Right("50%"),
                Width("fit-content")
            ),

            Def(FlexRow,
                Display("flex"),
                FlexDirection("row")
            ),

            Def(FlexEnd,
                Width("1rem"),
                AlignSelf("flex-right")
            ),

                Def(FlexStart,
                Width("7rem"),
                AlignSelf("flex-left")
            ),

            Def(HoverWhite,
                GetHoverRule("white")
            ),

            Def(FitContent,
                Width("fit-content")
            ),

            Def(H6,
                Height("6rem")
            ),

            Def(W2C,
                Width("2rem")
            ),

            Def(Wicon,
                Width("1.5rem")
            ),

            Def(Wentry,
                Width("12%")
            ),

            Def(W3C,
                Width("3rem")
            ),

            Def(W4C,
                Width("4rem")
            ),

            Def(W6C,
                Width("6rem")
            ),

            Def(W7C,
                Width("7rem")
            ),

             Def(W8C,
                Width("8rem")
            ),

            Def(W12C,
                Width("12rem")
            ),

            Def(FontSize1p5,
                FontSize("1.5rem")
            ),

            Def(FontSize3,
                FontSize("3rem")
            ),

            Def(Sidebar,
                Width("15%"),
                Height("100%"),
                Position("fixed"),
                ZIndex("1"),
                Top("0"),
                Left("0"),
                BackgroundColor("silver")
            ),

            Def(MainWindow,
                Width("100%"),
                Height("100%"),
                PaddingLeft("15%"),
                Position("fixed"),
                Top(""),
                Left("15%"),
                BackgroundColor("cadetblue")
            ),

            Def(P15P,
                 PaddingLeft("15%")
            ),

            Def(W15,
                Width("15%")
            ),

            Def(W25,
                Width("25%")
            ),

            Def(W33,
                Width("33%")
            ),

            Def(W50,
                Width("50%")
            ),

            Def(W75,
                Width("75%")
            ),

             Def(W100,
                Width("100%")
            ),

              Def(H100,
                Height("100%")
            ),

             Def(BCred,
                BackgroundColor("red")
            ),

              Def(BCMain,
                BackgroundColor("cadetblue")
            ),

              Def(BorderWhiteSolid,
                Border("solid"),
                BorderColor("linen")
            ),

             Def(BCMenu,
                BackgroundColor("burlywood")
            ),

              Def(Underline,
                TextDecoration("underline")
            ),

             Def(DInlineBlock,
                Display("inline-block")
            )
            

        );
        private Rule GetHoverRule(string color)
        {
            return Def("&:hover", BackgroundColor(color));
        }

        public static readonly Style Underline;

        public static readonly Style TileBase;
        public static readonly Style TileBlack;
        public static readonly Style TileWhite;
        public static readonly Style BorderBlack;
        public static readonly Style BorderWhite;


        public static readonly Style Absolute;
        public static readonly Style Relative;
        public static readonly Style HoverWhite;

        public static readonly Style BCblack;
        public static readonly Style BCMenu;
        public static readonly Style BCMain;
        public static readonly Style BCred;
        public static readonly Style TCblack;
        public static readonly Style TCred;
        public static readonly Style TCwhite;
        public static readonly Style TCgreen;

        public static readonly Style FontSize1p5;
        public static readonly Style FontSize3;

        public static readonly Style H6;
        public static readonly Style MP4;
        public static readonly Style MP8;
        public static readonly Style MR0P5;
        public static readonly Style M2;
        public static readonly Style MP2;
        public static readonly Style Ml6;
        public static readonly Style MX2;
        public static readonly Style MX1;
        public static readonly Style MY1;
        public static readonly Style MY2;
        public static readonly Style MY6;
        public static readonly Style MR2;
        public static readonly Style ML2;
        public static readonly Style ML8;
        public static readonly Style MT1;
        public static readonly Style MT2;
        public static readonly Style MB2;
        public static readonly Style MB2P5rem;
        
        public static readonly Style Wicon;
        public static readonly Style Wentry;


        public static readonly Style P4;

        public static readonly Style W2C;
        public static readonly Style W3C;
        public static readonly Style W4C;
        public static readonly Style W6C;
        public static readonly Style W7C;
        public static readonly Style W8C;
        public static readonly Style W12C;

        public static readonly Style P15P;

        public static readonly Style BorderWhiteSolid;
        public static readonly Style Sidebar;
        public static readonly Style W15;
        public static readonly Style W25;
        public static readonly Style W33;
        public static readonly Style W50;
        public static readonly Style W75;
        public static readonly Style W100;

        public static readonly Style H100;

        public static readonly Style Dropdown;
        public static readonly Style FlexRow;
        public static readonly Style FlexStart;
        public static readonly Style FlexEnd;

        public static readonly Style BaseBtn;
        public static readonly Style Btn;
        public static readonly Style AbortBtn;
        public static readonly Style SelectedBtn;

        public static readonly Style MainWindow;
        public static readonly Style MainMenuItem;
        public static readonly Style SubMenuItem;
        public static readonly Style SubMenuItemSelected;
        public static readonly Style TabNameTagNoWidth;
        public static readonly Style TabNameTagNoWidthMargin;
        
        public static readonly Style TabNameTag;
        public static readonly Style TabButtonSelected;
        public static readonly Style TabButton;
        public static readonly Style BtnSettings;
        
        public static readonly Style TabButtonSmall;
        public static readonly Style TabMenuItem;
        public static readonly Style TabMenuItemSelected;

        public static readonly Style Selected;

        public static readonly Style Fontcopperplate;
        public static readonly Style FontBrushscriptmt;

        public static readonly Style BorderW2;
        public static readonly Style BorderW4;
        public static readonly Style BorderedBox;
        public static readonly Style BorderedBoxPartialBase;
        public static readonly Style BorderedBoxPartial;
        public static readonly Style BorderedBoxPartialSelected;
        public static readonly Style BorderedBoxBlack;
        public static readonly Style BorderedBoxGreen;
        public static readonly Style WinBox;
        public static readonly Style BorderedBoxRed;
        public static readonly Style BorderedBoxPurple;
        public static readonly Style Score;
        public static readonly Style CardBackPartial;
        public static readonly Style CardEmptyBorderGreen;

        public static readonly Style CardRed;
        public static readonly Style CardBlack;
        public static readonly Style CardGreen;

        public static readonly Style AlignItemRight;
        public static readonly Style AlignItemCenter;

        public static readonly Style FitContent;

        public static readonly Style TextAlignR;
        public static readonly Style TextAlignC;

        public static readonly Style DInlineBlock;

        public static readonly Style R10;
    }

}
