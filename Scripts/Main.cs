using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class Main : Control
{
    private string _lang = "en";
    private string _page = "home";
    private int _caseIndex = 0;
    private readonly HashSet<string> _found = new();
    private readonly HashSet<string> _linked = new();
    private VBoxContainer _body = null!;
    private Label _title = null!;
    private Label _subtitle = null!;
    private readonly Color _bg = new("#090D13");
    private readonly Color _panel = new("#141C27");
    private readonly Color _muted = new("#9BA9B8");
    private readonly Color _red = new("#D56B72");
    private readonly Color _teal = new("#75B9C2");

    private readonly string[] _caseIds = { "001", "002", "003" };
    private readonly string[] _caseNames = { "The Last Message", "The Glass Room", "A Borrowed Hour" };
    private readonly string[] _caseNamesFa = { "آخرین پیام", "اتاق شیشه‌ای", "ساعت قرضی" };
    private readonly string[] _caseDescriptions = {
        "A missing-person mystery hidden inside a phone full of messages.",
        "A sealed archive, a broken clock, and three incompatible alibis.",
        "A midnight letter arrives one day before it was written."
    };
    private readonly string[] _caseDescriptionsFa = {
        "معمای ناپدیدشدن؛ حقیقت در میان پیام‌های یک تلفن پنهان شده است.",
        "یک آرشیو مهروموم‌شده، ساعتی شکسته و سه زمان‌بندی ناسازگار.",
        "نامه‌ای نیمه‌شب می‌رسد؛ یک روز پیش از آن‌که نوشته شده باشد."
    };

    private readonly (string id, string en, string fa, string detail, string detailFa)[] _clues = {
        ("phone", "Zoey's phone", "تلفن زویی", "The device stopped syncing at 21:14.", "همگام‌سازی دستگاه ساعت ۲۱:۱۴ متوقف شده است."),
        ("chat", "Group chat", "گفت‌وگوی گروهی", "Lily says Zoey went straight to the party.", "لیلی می‌گوید زویی مستقیم به مهمانی رفته است."),
        ("ticket", "Transit ticket", "بلیط حمل‌ونقل", "A ticket was validated far from the party.", "بلیطی در مکانی دور از مهمانی ثبت شده است."),
        ("photo", "Blurred photograph", "عکس تار", "A reflection shows a second person.", "در انعکاس تصویر، شخص دیگری دیده می‌شود."),
        ("note", "Folded note", "یادداشت تاخورده", "The note mentions the old observatory.", "در یادداشت به رصدخانه قدیمی اشاره شده است.")
    };

    public override void _Ready()
    {
        BuildShell();
        ShowPage();
    }

    private string T(string en, string fa) => _lang == "fa" ? fa : en;
    private void BuildShell()
    {
        var root = new VBoxContainer { AnchorRight = 1, AnchorBottom = 1, OffsetLeft = 26, OffsetTop = 24, OffsetRight = -26, OffsetBottom = -24 };
        root.AddThemeConstantOverride("separation", 16);
        AddChild(root);

        var top = new HBoxContainer();
        top.AddChild(MakeLabel("CZ / 09", 18, _red));
        top.AddChild(new Control { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });
        var lang = MakeButton("EN  /  فارسی", () => { _lang = _lang == "en" ? "fa" : "en"; ShowPage(); }, false);
        top.AddChild(lang);
        root.AddChild(top);

        _title = MakeLabel("CASE ZERO", 38, Colors.White);
        root.AddChild(_title);
        _subtitle = MakeLabel("CONFIDENTIAL INVESTIGATION UNIT", 12, _muted);
        root.AddChild(_subtitle);

        var line = new ColorRect { Color = new Color("#34404D"), CustomMinimumSize = new Vector2(0, 1) };
        root.AddChild(line);

        var scroll = new ScrollContainer { SizeFlagsVertical = Control.SizeFlags.ExpandFill, HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        _body = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        _body.AddThemeConstantOverride("separation", 14);
        scroll.AddChild(_body);
        root.AddChild(scroll);

        var nav = new HBoxContainer();
        nav.AddChild(MakeButton(T("CASES", "پرونده‌ها"), () => Navigate("home"), true));
        nav.AddChild(MakeButton(T("BOARD", "شواهد"), () => Navigate("board"), true));
        nav.AddChild(MakeButton(T("PEOPLE", "افراد"), () => Navigate("people"), true));
        nav.AddChild(MakeButton(T("REPORT", "گزارش"), () => Navigate("report"), true));
        root.AddChild(nav);
    }

    private void Navigate(string page) { _page = page; ShowPage(); }

    private void ShowPage()
    {
        foreach (Node child in _body.GetChildren()) child.QueueFree();
        _title.Text = T("CASE ZERO", "پرونده صفر");
        _subtitle.Text = T("CONFIDENTIAL INVESTIGATION UNIT", "واحد تحقیقات محرمانه");

        switch (_page)
        {
            case "home": ShowHome(); break;
            case "case": ShowCase(); break;
            case "board": ShowBoard(); break;
            case "people": ShowPeople(); break;
            case "report": ShowReport(); break;
            case "chat": ShowChat(); break;
            default: ShowHome(); break;
        }
    }

    private void ShowHome()
    {
        AddEyebrow(T("ACTIVE DOSSIERS / 03", "پرونده‌های فعال / ۰۳"));
        AddText(T("Some stories leave evidence. Others leave questions.", "بعضی داستان‌ها مدرک به‌جا می‌گذارند؛ بعضی فقط سؤال."), 21, Colors.White);
        for (int i = 0; i < _caseIds.Length; i++)
        {
            int idx = i;
            var card = MakeCard();
            ((VBoxContainer)card.GetChild(0)).AddChild(MakeLabel("CASE FILE   /   " + _caseIds[i], 12, _red));
            ((VBoxContainer)card.GetChild(0)).AddChild(MakeLabel(_lang == "fa" ? _caseNamesFa[i] : _caseNames[i], 23, Colors.White));
            ((VBoxContainer)card.GetChild(0)).AddChild(MakeLabel(_lang == "fa" ? _caseDescriptionsFa[i] : _caseDescriptions[i], 14, _muted));
            ((VBoxContainer)card.GetChild(0)).AddChild(MakeButton(T("OPEN DOSSIER  →", "باز کردن پرونده  ←"), () => { _caseIndex = idx; _page = "case"; ShowPage(); }, false));
            _body.AddChild(card);
        }
        AddEyebrow(T("FIELD NOTE", "یادداشت محرمانه"));
        AddText(T("No conclusion is final until the evidence agrees.", "تا زمانی که مدارک با هم سازگار نشوند، هیچ نتیجه‌ای قطعی نیست."), 15, _teal);
    }

    private void ShowCase()
    {
        var i = _caseIndex;
        AddEyebrow("CASE " + _caseIds[i] + "   /   " + T("OPEN", "باز"));
        AddText(_lang == "fa" ? _caseNamesFa[i] : _caseNames[i], 30, Colors.White);
        AddText(_lang == "fa" ? _caseDescriptionsFa[i] : _caseDescriptions[i], 15, _muted);
        AddText(T("CASE BRIEF", "خلاصه پرونده"), 14, _red);
        AddText(T("Zoey's last known evening is reconstructed through messages, a transit record and a photograph. Your task is to separate what witnesses believe from what the evidence proves.", "آخرین شب زویی از طریق پیام‌ها، یک سابقه رفت‌وآمد و یک عکس بازسازی می‌شود. وظیفه تو جدا کردن باور شاهدان از چیزی است که مدارک واقعاً ثابت می‌کنند."), 16, Colors.White);
        AddText(T("DIGITAL DEVICE", "دستگاه دیجیتال"), 13, _teal);
        AddText(T("The recovered phone contains a group conversation and an unsent draft. Open the chat to inspect the timeline.", "تلفن بازیابی‌شده شامل گفت‌وگوی گروهی و پیش‌نویسی ارسال‌نشده است. برای بررسی زمان‌بندی، گفت‌وگو را باز کن."), 14, _muted);
        _body.AddChild(MakeButton(T("OPEN GROUP CHAT", "باز کردن گفت‌وگوی گروهی"), () => Navigate("chat"), false));
        _body.AddChild(MakeButton(T("INSPECT EVIDENCE", "بررسی مدارک"), () => Navigate("board"), false));
        _body.AddChild(MakeButton(T("VIEW PEOPLE", "مشاهده افراد"), () => Navigate("people"), false));
    }

    private void ShowChat()
    {
        AddEyebrow(T("RECOVERED DEVICE / CHAT LOG", "دستگاه بازیابی‌شده / گزارش گفت‌وگو"));
        AddText("GROUPCHAT_04", 24, Colors.White);
        ChatBubble("LILY", T("Zoey said she was heading straight to the party.", "زویی گفت مستقیم به مهمانی می‌رود."), false, "20:42");
        ChatBubble("BETTY", T("That is not what she told me.", "او به من چیز دیگری گفت."), true, "20:47");
        ChatBubble("NOAH", T("Did anyone see her arrive?", "کسی رسیدنش را دید؟"), false, "20:51");
        ChatBubble("LILY", T("I only saw her coat near the entrance.", "فقط پالتویش را نزدیک ورودی دیدم."), true, "20:54");
        ChatBubble("SYSTEM", T("Message attachment recovered: transit_ticket.jpg", "پیوست بازیابی شد: transit_ticket.jpg"), false, "21:03");
        AddText(T("The chat does not prove where Zoey went. It records what each person claims.", "این گفت‌وگو محل رفتن زویی را ثابت نمی‌کند؛ فقط ادعای هر شخص را ثبت می‌کند."), 14, _teal);
        _body.AddChild(MakeButton(T("ADD CHAT AS EVIDENCE", "ثبت گفت‌وگو به‌عنوان مدرک"), () => { _found.Add("chat"); Navigate("board"); }, false));
    }

    private void ChatBubble(string who, string message, bool right, string time)
    {
        var box = MakeCard();
        ((VBoxContainer)box.GetChild(0)).AddChild(MakeLabel(who + "   ·   " + time, 11, right ? _teal : _red));
        ((VBoxContainer)box.GetChild(0)).AddChild(MakeLabel(message, 16, Colors.White));
        _body.AddChild(box);
    }

    private void ShowBoard()
    {
        AddEyebrow(T("EVIDENCE WALL / INTERACTIVE", "دیوار شواهد / تعاملی"));
        AddText(T("Connect what belongs together.", "چیزهایی را که به هم مربوط‌اند متصل کن."), 23, Colors.White);
        AddText(T("Select a clue to inspect it. Mark it found, then compare it with the other records.", "برای بررسی، یک سرنخ را انتخاب کن. آن را ثبت کن و با مدارک دیگر مقایسه کن."), 14, _muted);
        foreach (var clue in _clues)
        {
            var c = clue;
            bool found = _found.Contains(c.id);
            var card = MakeCard();
            ((VBoxContainer)card.GetChild(0)).AddChild(MakeLabel(found ? "✓  " + (_lang == "fa" ? c.fa : c.en) : "＋  " + (_lang == "fa" ? c.fa : c.en), 18, found ? _teal : Colors.White));
            ((VBoxContainer)card.GetChild(0)).AddChild(MakeLabel(_lang == "fa" ? c.detailFa : c.detail, 13, _muted));
            ((VBoxContainer)card.GetChild(0)).AddChild(MakeButton(found ? T("RECORDED", "ثبت‌شده") : T("ADD TO CASE FILE", "افزودن به پرونده"), () => { _found.Add(c.id); ShowPage(); }, found));
            _body.AddChild(card);
        }
        AddText(T("CONNECTION LOG", "گزارش ارتباط‌ها"), 14, _red);
        AddText(T("A connection is a hypothesis until another clue supports it. Compare the chat with the transit ticket and photograph.", "ارتباط تا زمانی که سرنخ دیگری پشتیبانی‌اش نکند، یک فرضیه است. گفت‌وگو را با بلیط و عکس مقایسه کن."), 14, _muted);
        _body.AddChild(MakeButton(T("RECORD A CONNECTION", "ثبت یک ارتباط"), () => { _linked.Add("chat-ticket"); ShowPage(); }, _linked.Contains("chat-ticket")));
        if (_linked.Contains("chat-ticket")) AddText(T("LINK SAVED: CHAT ↔ TRANSIT TICKET", "ارتباط ثبت شد: گفت‌وگو ↔ بلیط"), 13, _teal);
    }

    private void ShowPeople()
    {
        AddEyebrow(T("PERSONS OF INTEREST", "افراد مرتبط با پرونده"));
        AddText(T("Four accounts. Different versions.", "چهار روایت؛ چهار نسخه متفاوت."), 22, Colors.White);
        Person("LILY VALE", T("Friend · Last message sender", "دوست · فرستنده آخرین پیام"), T("Says Zoey went to the party.", "می‌گوید زویی به مهمانی رفته است."));
        Person("BETTY PARK", T("Neighbour · Witness", "همسایه · شاهد"), T("Disputes Lily's timeline.", "زمان‌بندی لیلی را رد می‌کند."));
        Person("NOAH REED", T("Colleague · Contact", "همکار · آشنا"), T("Claims he did not meet Zoey that night.", "ادعا می‌کند آن شب زویی را ندیده است."));
        Person("MARA QUINN", T("Transit clerk · Record holder", "کارمند حمل‌ونقل · دارنده سابقه"), T("Can explain the ticket timestamp.", "می‌تواند زمان ثبت بلیط را توضیح دهد."));
    }

    private void Person(string name, string role, string note)
    {
        var card = MakeCard();
        ((VBoxContainer)card.GetChild(0)).AddChild(MakeLabel(name, 19, Colors.White));
        ((VBoxContainer)card.GetChild(0)).AddChild(MakeLabel(role, 12, _red));
        ((VBoxContainer)card.GetChild(0)).AddChild(MakeLabel(note, 14, _muted));
        _body.AddChild(card);
    }

    private void ShowReport()
    {
        AddEyebrow(T("INVESTIGATOR'S REPORT", "گزارش کارآگاه"));
        AddText(T("Build your conclusion from the record.", "نتیجه‌گیری را بر اساس مدارک ثبت‌شده بساز."), 22, Colors.White);
        AddText(T("Evidence collected: ", "مدارک جمع‌آوری‌شده: ") + _found.Count + " / " + _clues.Length, 16, _teal);
        AddText(T("Connections recorded: ", "ارتباط‌های ثبت‌شده: ") + _linked.Count, 16, _teal);
        AddText(T("Current theory", "فرضیه فعلی"), 14, _red);
        AddText(T("The timeline contains a contradiction. The chat records a claim, while the transit record may establish a location. Do not accuse a person based on one statement alone.", "در زمان‌بندی تناقض وجود دارد. گفت‌وگو یک ادعا را ثبت می‌کند، درحالی‌که سابقه رفت‌وآمد ممکن است مکان را مشخص کند. تنها بر اساس یک گفته، کسی را متهم نکن."), 15, Colors.White);
        _body.AddChild(MakeButton(T("SUBMIT PRELIMINARY REPORT", "ثبت گزارش اولیه"), () => ShowDialog(T("REPORT SAVED", "گزارش ذخیره شد"), T("Your current theory has been saved locally.", "فرضیه فعلی به‌صورت محلی ذخیره شد.")), false));
        AddText(T("More case chapters will unlock as evidence is reviewed.", "با بررسی مدارک، فصل‌های بعدی پرونده باز خواهند شد."), 13, _muted);
    }

    private void ShowDialog(string title, string message)
    {
        var d = new AcceptDialog { Title = title, DialogText = message };
        AddChild(d);
        d.PopupCentered();
    }

    private void AddEyebrow(string text) => _body.AddChild(MakeLabel(text, 12, _red));
    private void AddText(string text, int size, Color color) => _body.AddChild(MakeLabel(text, size, color));

    private Label MakeLabel(string text, int size, Color color)
    {
        var label = new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart, Modulate = color, SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        label.AddThemeFontSizeOverride("font_size", size);
        return label;
    }

    private Button MakeButton(string text, Action action, bool secondary)
    {
        var b = new Button { Text = text, CustomMinimumSize = new Vector2(0, 54), SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, FocusMode = Control.FocusModeEnum.None };
        b.AddThemeFontSizeOverride("font_size", 14);
        var normal = new StyleBoxFlat { BgColor = secondary ? new Color("#18222E") : new Color("#29313D"), BorderColor = secondary ? new Color("#354454") : _red, BorderWidthBottom = 1, BorderWidthTop = 1, BorderWidthLeft = 1, BorderWidthRight = 1, CornerRadiusTopLeft = 9, CornerRadiusTopRight = 9, CornerRadiusBottomLeft = 9, CornerRadiusBottomRight = 9, ContentMarginLeft = 12, ContentMarginRight = 12 };
        var hover = (StyleBoxFlat)normal.Duplicate(); hover.BgColor = new Color("#35404D");
        b.AddThemeStyleboxOverride("normal", normal); b.AddThemeStyleboxOverride("hover", hover);
        b.Pressed += () => action();
        return b;
    }

    private PanelContainer MakeCard()
    {
        var panel = new PanelContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ShrinkBegin
        };
        var style = new StyleBoxFlat
        {
            BgColor = new Color("#111923"),
            BorderColor = new Color("#2B3A49"),
            BorderWidthTop = 1,
            BorderWidthBottom = 1,
            BorderWidthLeft = 1,
            BorderWidthRight = 1,
            CornerRadiusTopLeft = 14,
            CornerRadiusTopRight = 14,
            CornerRadiusBottomLeft = 14,
            CornerRadiusBottomRight = 14,
            ContentMarginLeft = 16,
            ContentMarginRight = 16,
            ContentMarginTop = 14,
            ContentMarginBottom = 14
        };
        panel.AddThemeStyleboxOverride("panel", style);
        var content = new VBoxContainer();
        content.AddThemeConstantOverride("separation", 9);
        panel.AddChild(content);
        return panel;
    }}
