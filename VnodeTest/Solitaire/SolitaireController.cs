using ACL.ES;
using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.General.Account;
using VnodeTest.BC.Solitaire;
using VnodeTest.Solitaire.GameEntities;
using static ACL.UI.React.DOM;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;


namespace VnodeTest.Solitaire
{
    public partial class SolitaireController
    {
        private GameID GameID /*=> LastPlayedgame == default ? SolitaireProjection.GetGameID(AccountID) : LastPlayedgame*/;

        public AccountProjection AccountProjection { get; }
        private AccountID AccountID { get; }

        private GameID LastPlayedgame => SolitaireProjection.GetLastPlayedGame(AccountID);
        private Gameboard GameBoard => GameID != default ? SolitaireProjection[GameID].GameBoard : null;
        private Card Selected;
        private ScoreTimespan ScoreOfTimespan = ScoreTimespan.Alltime;
        private bool HasClosedCards => GameBoard != null ? GameBoard.GamePiles.Where(x => x.Any(c => !c.IsFaceUp)).Any() : true;

        private SolitaireProjection SolitaireProjection;

        public SolitaireController(SolitaireProjection solitaireProjection, AccountProjection accountProjection, AccountID accountID)
        {
            SolitaireProjection = solitaireProjection;
            AccountProjection = accountProjection;
            AccountID = accountID;
        }

        public VNode Render()
        {
            void startNewGame()
            {
                BC.Solitaire.Solitaire.Commands.OpenGame(GameID = GameID.Create(), AccountID);
                BC.Solitaire.Solitaire.Commands.JoinGame(GameID, AccountID);
            }

            void joinOldGame()
            {
                GameID = LastPlayedgame;
                BC.Solitaire.Solitaire.Commands.JoinGame(GameID, AccountID);
            }

            VNode highscore(IEnumerable<GameEntry> gameEntries, string title)
            {
                return Div(
                     Styles.TabNameTag & Styles.FitContent & Styles.Fontcopperplate & Styles.Underline & Styles.MY2,
                    Text(title),
                    Fragment(gameEntries.ToList().Select(e =>
                        Row(
                            Styles.Fontcopperplate,
                            Text($"{gameEntries.ToList().IndexOf(e) + 1}", Styles.MX2 & Styles.W3C),
                            Text(e.FinalScore.ToString(), Styles.W3C & Styles.TextAlignR),
                            Text(AccountProjection[e.Player].Username, Styles.MX2 & Styles.W4C & Styles.TextAlignR)
                ))));
            }

            if (GameID == default)
            {
                VNode continueLastGame = null;
                if (LastPlayedgame != default)
                    continueLastGame = Text("Continue Last Game", Styles.TabButton & Styles.W12C, () => joinOldGame());
                return Div(
                    Row(
                        continueLastGame,
                        Text("Start New Game", Styles.TabButton, () => startNewGame())
                    ),
                    //dropdown here
                    Div(
                        Styles.P4 & Styles.TabNameTagNoWidth & Styles.W8C,
                        DropdownComponent<ScoreTimespan>.Render(GetScoreTimeSpan(), a => ScoreOfTimespan = a, ScoreOfTimespan.ToString(), s => Text(s.ToString()))
                    ),
                    Row(
                       highscore(SolitaireProjection.GetPersonalHighscore(ScoreOfTimespan, AccountID), "Personal Highscore"),
                       highscore(SolitaireProjection.GetGlobalHighscore(ScoreOfTimespan), "Global Highscore")
                    )
                );
            }
            return RenderGameboard();
        }

        private IEnumerable<ScoreTimespan> GetScoreTimeSpan()
        {
            return Enumerable.Empty<ScoreTimespan>()
                .Append(ScoreTimespan.Alltime)
                .Append(ScoreTimespan.ThreeMonths)
                .Append(ScoreTimespan.TwelveMonths);
        }

        private VNode RenderGameboard()
        {
            return Div(
                Text("Surrender", Styles.TabButtonSelected, () => { BC.Solitaire.Solitaire.Commands.EndGame(GameID, AccountID, GetCorrectScore()); GameID = default; }),
                Row(
                    Row(
                        Styles.FitContent & Styles.W33,
                        Div(() => GameBoard.Tableau.NextCard(), RenderCardback(Styles.CardGreen)),
                        GameBoard.Tableau.TableauGraveyard.IsEmpty ? Div(Styles.CardEmptyBorderGreen) : RenderCard(GameBoard.Tableau.TableauGraveyard.Peek())
                    ),
                    RenderFoundationPiles(),
                    RenderScore()
                ),
                RenderGamePiles(),
                RenderWin()
            );
        }

        private int GetCorrectScore()
        {
            if (HasClosedCards)
                return GameBoard.Score;
            return 520 - GameBoard.Tableau.GraveyardTurnedOverCounter * 20;
        }

        private VNode RenderWin()
        {
            if (HasClosedCards)
                return null;
            return Div(
                Styles.TCgreen & Styles.WinBox & Styles.AlignItemCenter & Styles.MT2,
                Text($"You Won!", Styles.FontSize3),
                Text($"Score: {GetCorrectScore()}", Styles.FontSize3)
                );
        }

        private VNode RenderFoundationPiles()
        {
            VNode renderEmptyFoundation(FoundationStack target, Style color, string title = "Deck")
            {
                var div = RenderCardback(color, title);
                div.OnClick = () => { target.PushToEmptyStack(GameBoard, Selected); Selected = null; };
                return div;
            }

            VNode renderFoundation(FoundationStack foundation, Style color, string pip) =>
                foundation.Count != 0
                    ? RenderCard(foundation.Peek())
                    : renderEmptyFoundation(foundation, color, pip);

            return Row(
                renderFoundation(GameBoard.Foundations.Club, Styles.CardBlack, "Club"),
                renderFoundation(GameBoard.Foundations.Spade, Styles.CardBlack, "Spade"),
                renderFoundation(GameBoard.Foundations.Heart, Styles.CardRed, "Heart"),
                renderFoundation(GameBoard.Foundations.Diamond, Styles.CardRed, "Diamond")
            );
        }

        public VNode RenderScore()
        {
            return Row(
                Styles.TabNameTagNoWidth & Styles.Ml6,
                Text($"Score:", Styles.W3C),
                Text(GameBoard.Score.ToString(), Styles.TextAlignR & Styles.W3C)
            );
        }

        private VNode RenderGamePiles()
        {
            VNode renderGamePile(CardStack stack)
            {
                //full pile
                if (stack.Count != 0)
                    return Col(
                        Fragment(stack.Reverse().Take(stack.Count - 1).Select(RenderOverlappedCard)),
                        RenderCard(stack.Peek())
                    );

                //empty pile
                var emptyStack = Div(Styles.CardEmptyBorderGreen);
                emptyStack.OnClick = () => { stack.PushToEmptyStack(GameBoard, Selected); Selected = null; };
                return emptyStack;
            }
            return Row(GameBoard.GamePiles.Select(p => renderGamePile(p)));
        }

        private VNode RenderCard(Card card)
        {
            var boxStyle = card == Selected ? Styles.BorderedBoxPurple : Styles.BorderedBoxBlack;
            var cardTextColorStyle = GetCardTextColorStyle(card);
            var cardSprite = GetCardValueSprite(card);
            var pipSprite = GetPipValueSprite(card);

            if (!card.IsFaceUp)
                return Div(() => card.IsFaceUp = true, RenderCardback(Styles.CardGreen, "Click me!"));

            return Div(
                cardTextColorStyle & Styles.W4C & Styles.MP2 & boxStyle,
                () => ClickCard(card),
                Row(
                    Styles.W4C,
                    Text($"{cardSprite}", cardTextColorStyle & Styles.W2C),
                    Text($"{pipSprite}", cardTextColorStyle & Styles.TextAlignR & Styles.W2C)
                ),
                Text($"{cardSprite}", cardTextColorStyle & Styles.TextAlignC & Styles.W4C & Styles.FontSize3),
                Row(
                    Styles.W4C,
                    Text($"{pipSprite}", cardTextColorStyle & Styles.W2C),
                    Text($"{cardSprite}", cardTextColorStyle & Styles.TextAlignR & Styles.W2C)
                )
            );
        }

        private Style GetCardTextColorStyle(Card card)
        {
            if (card.Color == Card.ColorValue.Black)
                return Styles.TCblack;
            return Styles.TCred;
        }

        private string GetCardValueSprite(Card card)
        {
            return card.CardValue switch
            {
                CardValue.Ace => "A",
                CardValue.Two => "2",
                CardValue.Three => "3",
                CardValue.Four => "4",
                CardValue.Five => "5",
                CardValue.Six => "6",
                CardValue.Seven => "7",
                CardValue.Eight => "8",
                CardValue.Nine => "9",
                CardValue.Ten => "10",
                CardValue.Jack => "J",
                CardValue.Queen => "Q",
                CardValue.King => "K",
                _ => "0",
            };
        }

        private string GetPipValueSprite(Card card)
        {
            return card.PipValue switch
            {
                PipValue.Club => "♣",
                PipValue.Spade => "♠",
                PipValue.Heart => "♥",
                PipValue.Diamond => "♦",
                _ => "0",
            };
        }

        public void ClickCard(Card card)
        {
            if (Selected == null)
                Selected = card;
            else if (Selected == card)
                Selected = null;
            else
            {
                GameBoard.GetStack(card).TryPush(Selected, GameBoard.GetStack(Selected));
                Selected = null;
            }
        }

        private VNode RenderOverlappedCard(Card card)
        {
            Style cardStyle = card == Selected ? Styles.BorderedBoxPartialSelected : Styles.BorderedBoxPartial;
            var cardTextColoStyle = GetCardTextColorStyle(card);

            if (!card.IsFaceUp)
                return Div(
                    Styles.CardBackPartial & Styles.W4C & Styles.MP2,
                    Text("XXXXX", Styles.TextAlignC & Styles.W4C)
                );

            return Row(
                Styles.W4C & cardStyle & Styles.MP2,
                () => ClickCard(card),
                Text($"{GetCardValueSprite(card)}", cardTextColoStyle & Styles.W2C),
                Text($"{GetPipValueSprite(card)}", cardTextColoStyle & Styles.TextAlignR & Styles.W2C)
            );
        }

        public static VNode RenderCardback(Style color, string title = "Deck")
        {
            return Div(
                Styles.BorderedBox & Styles.W4C & Styles.MP2 & color,
                Text("XXXXX", Styles.TextAlignC & Styles.W4C),
                Text("XXXXX", Styles.TextAlignC & Styles.W4C),
                Text(title, Styles.W4C & Styles.TextAlignC),
                Text("XXXXX", Styles.TextAlignC & Styles.W4C),
                Text("XXXXX", Styles.TextAlignC & Styles.W4C)
            );
        }
    }
}
