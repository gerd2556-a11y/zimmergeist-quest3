using System.Collections.Generic;

namespace OfflinePet.Dialogue
{
    public interface IPetDialogueService
    {
        string GetGreeting(string petName, string personality);
        string GetFeedingReaction(string petName, string treatName, string personality);
        string GetPlayReaction(string petName, string toyName, string personality);
        string GetRestReaction(string petName, string personality);
        string GetPettingReaction(string petName, string personality);
        string GetVoiceCommandReaction(string petName, string commandKey, string personality);
        string GetMiniTaskPrompt(string taskType);
        string GetMiniTaskSuccess(string petName);
        string GetBoundaryAlert();
    }

    public class LocalPetDialogueService : IPetDialogueService
    {
        private static readonly System.Random Rnd = new System.Random();

        private static readonly Dictionary<string, string[]> Greetings = new Dictionary<string, string[]>
        {
            ["Curious"] = new[]
            {
                "Hallo! Schön, dich wiederzusehen! Was erkunden wir heute im Zimmer?",
                "Piep! Du bist wieder da! Ich habe mich schon gefragt, was du vorhast.",
                "*schwebt neugierig heran* Hallo! Ich freue mich auf unsere gemeinsame Zeit."
            },
            ["Calm"] = new[]
            {
                "Hallo. Schön, dass du da bist. Machen wir es uns gemütlich.",
                "*glimmt sanft auf* Willkommen zurück. Atme erst einmal tief durch.",
                "Hallo mein Freund. Schön, in deiner ruhigen Gesellschaft zu sein."
            },
            ["Playful"] = new[]
            {
                "Juhu! Du bist da! Lass uns gleich etwas Schönes spielen!",
                "*hüpft freudig in der Luft* Hallo! Ich bin voller Tatendrang!",
                "Hallo! Ich habe schon ungeduldig auf dich gewartet!"
            }
        };

        private static readonly Dictionary<string, string[]> Feedings = new Dictionary<string, string[]>
        {
            ["Curious"] = new[]
            {
                "Mmh, das schmeckt interessant! Danke für die leckere Beere!",
                "*schnuppert neugierig und knabbert genüßlich* Vielen Dank!",
                "Oh, was für ein feiner Snack! Mein Energielevel steigt schon."
            },
            ["Calm"] = new[]
            {
                "Vielen Dank für diese sanfte Stärkung.",
                "*frisst langsam und genießt jeden Bissen* Danke, das tut gut.",
                "Eine willkommene Mahlzeit. Danke, dass du dich so lieb kümmerst."
            },
            ["Playful"] = new[]
            {
                "Nom nom nom! Das war superlecker! Hast du noch eins?",
                "*wirbelt einmal herum vor Freude* Köstlich! Danke!",
                "Mjam! Genau das, was ich gebraucht habe!"
            }
        };

        private static readonly Dictionary<string, string[]> Playings = new Dictionary<string, string[]>
        {
            ["Curious"] = new[]
            {
                "Faszinierend, wie der Lichtball schwebt! Schau mal!",
                "*folgt dem Spielzeug aufmerksam mit den Augen* Das macht Spaß!",
                "Tolles Spielzeug! Probieren wir gleich noch eine Runde aus!"
            },
            ["Calm"] = new[]
            {
                "Ein angenehmes Spiel. Es entspannt mich sehr.",
                "*lächelt ruhig und stupst das Spielzeug sachte an* Danke.",
                "Ruhiges Spielen in deinem Zimmer ist die beste Pause vom Alltag."
            },
            ["Playful"] = new[]
            {
                "Haha, fang mich oder den Ball! Das ist genial!",
                "*vollführt eine fröhliche Drehung* Nochmal, bitte noch eine Runde!",
                "Das macht riesigen Spaß mit dir!"
            }
        };

        private static readonly Dictionary<string, string[]> Restings = new Dictionary<string, string[]>
        {
            ["Curious"] = new[]
            {
                "*gähnt leise und rollt sich sanft zusammen* Eine kleine Rast tut gut...",
                "Ich bleibe hier friedlich vor dir liegen und schließe kurz die Augen.",
                "Puh, so viele Entdeckungen. Ein kurzes Schläfchen lädt meine Kraft auf."
            },
            ["Calm"] = new[]
            {
                "*senkt sanft die Helligkeit und atmet gleichmäßig* Friedliche Ruhe.",
                "Alles ist still und geborgen. Ruh dich gerne auch ein wenig aus.",
                "Hier vor dir ist der sicherste Platz zum Träumen."
            },
            ["Playful"] = new[]
            {
                "*lässt sich sanft absinken* Puh, kurz verschnaufen... aber bald spielen wir weiter!",
                "Ein Powernap für neue Abenteuer! Bis gleich...",
                "*schlummert zufrieden mit einem Lächeln*"
            }
        };

        private static readonly Dictionary<string, string[]> Pettings = new Dictionary<string, string[]>
        {
            ["Curious"] = new[]
            {
                "*lehnt sich sanft an deine Hand* Das kitzelt angenehm warm!",
                "*strahlt leicht auf* Deine Berührung ist sehr beruhigend.",
                "*schließt die Augen und genießt das Streicheln*"
            },
            ["Calm"] = new[]
            {
                "*glimmt in sanftem Goldton* Vielen Dank, das tut gut.",
                "Eine wohlige Wärme. Ich schätze deine Freundlichkeit sehr.",
                "*atmet ruhig aus und entspannt sich vollkommen*"
            },
            ["Playful"] = new[]
            {
                "*schnurrt leise wie eine kleine Sternenkatze* Noch mehr Krauleinheiten!",
                "*macht ein zufriedenes Geräusch* Du bist der beste Freund!",
                "*schwebt vor Wonne ganz leicht auf und ab*"
            }
        };

        public string GetGreeting(string petName, string personality) => Pick(Greetings, personality);
        public string GetFeedingReaction(string petName, string treatName, string personality) => Pick(Feedings, personality);
        public string GetPlayReaction(string petName, string toyName, string personality) => Pick(Playings, personality);
        public string GetRestReaction(string petName, string personality) => Pick(Restings, personality);
        public string GetPettingReaction(string petName, string personality) => Pick(Pettings, personality);

        public string GetVoiceCommandReaction(string petName, string commandKey, string personality)
        {
            switch (commandKey.ToLowerInvariant())
            {
                case "hallo":
                case "komm her":
                    return $"{petName} blickt aufmerksam zu dir und schwebt nah vor dich.";
                case "füttern":
                    return $"{petName} schaut voller Vorfreude auf eine Leckerei!";
                case "spielen":
                    return $"{petName} freut sich auf ein entspanntes Spiel im Zimmer.";
                case "ruh dich aus":
                    return $"{petName} entspannt sich und schließt friedlich die Augen.";
                case "freu dich":
                    return $"{petName} vollführt eine fröhliche Drehung und leuchtet warm auf!";
                case "stopp":
                    return $"{petName} hält sofort ruhig an und wartet geduldig auf dich.";
                case "menü":
                case "pause":
                    return "Pausenmenü geöffnet. Das Spiel wartet auf deine Auswahl.";
                default:
                    return $"{petName} hat das aufmerksam wahrgenommen.";
            }
        }

        public string GetMiniTaskPrompt(string taskType)
        {
            return "Merkaufgabe: Merke dir die Farben, die Lumi dir gleich nacheinander zeigt!";
        }

        public string GetMiniTaskSuccess(string petName)
        {
            return $"Großartig gelöst! {petName} freut sich riesig über deine Aufmerksamkeit.";
        }

        public string GetBoundaryAlert()
        {
            return "Sicherheitshinweis: Bitte bleibe im sicheren Spielbereich deines Zimmers. Das Haustier wartet vor dir.";
        }

        private string Pick(Dictionary<string, string[]> dict, string personality)
        {
            if (!dict.TryGetValue(personality, out var list) || list == null || list.Length == 0)
            {
                dict.TryGetValue("Curious", out list);
            }

            if (list == null || list.Length == 0) return "Lumi lächelt freundlich.";
            return list[Rnd.Next(list.Length)];
        }
    }
}
