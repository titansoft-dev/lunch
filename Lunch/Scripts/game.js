(function (i, s, o, g, r, a, m) {
    i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
        (i[r].q = i[r].q || []).push(arguments)
    }, i[r].l = 1 * new Date(); a = s.createElement(o),
    m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
})(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');
ga('create', 'UA-73339527-2', 'auto');

var userName = $("#name").val();
var isOver = false;
var gameName = "進擊的射擊";
var shift = 3, canvasHeight = 200, canvasWidth = 350, sec = 30;
var target = null, resources = null, bck = null;
var bullets = [];
var renderer = PIXI.autoDetectRenderer(canvasWidth, canvasHeight);
renderer.autoResize = true;
var isMobile = navigator.userAgent.toLocaleLowerCase().indexOf("mobile") > -1;
if (isMobile) {
    userName = getParameterByName("userName");
    if (screen.height > screen.width) {
        renderer.resize(screen.height, screen.width);
    } else {
        renderer.resize(screen.width, screen.height);
    }
    canvasWidth = renderer.width;
    canvasHeight = renderer.height;
    shift = canvasWidth * 0.005;

    $("#game").css({
        "position": "absolute",
        "left": 0,
        "top": 0
    });
}

$("#game").append(renderer.view);
var stage = new PIXI.Container();
var loader = PIXI.loader;
loader.add("backgroud", "/Content/images/backgroud.jpg");
loader.add("pic", "/Content/images/11.gif");
loader.add("hit", "/Content/images/26.gif");
loader.add("start", "/Content/images/start.png");
loader.add("volumeup", "/Content/images/volume-up.png");
loader.add("volumeoff", "/Content/images/volume-off.png");
loader.add("bckmusic", "/Content/musics/bckmusic.mp3");
loader.add("bullet", "/Content/musics/bullet.wav");
loader.add("backlash", "/Content/musics/backlash.wav");

loader.on("progress", function () {
    var progressWidth = canvasWidth * 0.85;
    var progressHeight = canvasHeight * 0.025;
    var originalX = (canvasWidth - progressWidth) / 2;
    var originalY = canvasHeight * 0.6;
    var lineWidth = canvasHeight * 0.01;

    var loadBar = stage.getChildByName("loadBar");
    if (!loadBar) {
        loadBar = new PIXI.Graphics();
        loadBar.name = "loadBar";
        loadBar.lineStyle(lineWidth, 0xFFFF00, 1);
        loadBar.drawRect(originalX, originalY, progressWidth, progressHeight);
        stage.addChild(loadBar);
    }

    loadBar.lineStyle(progressHeight - lineWidth, 0xFF0000, 1);
    loadBar.moveTo(originalX, originalY + lineWidth);
    loadBar.lineTo(originalX + (progressWidth * this.progress / 100), originalY + lineWidth);
    loadBar.endFill();

    renderer.render(stage);
});
loader.on("complete", function () {
    stage.removeChild(stage.getChildByName("loadBar"));
    resources = this.resources;
    createjs.Sound.registerSound(resources.bckmusic.url, "player");
    createjs.Sound.registerSound(resources.bullet.url, "bullet");
    createjs.Sound.registerSound(resources.backlash.url, "backlash");

    bck = new PIXI.Sprite(resources.backgroud.texture);
    bck.width = canvasWidth; bck.height = canvasHeight;
    bck.alpha = 0.6;
    stage.addChild(bck);
    setGameNameTxet();
    renderer.render(stage);
    gameStartAnimation();
});
loader.load();

function startGame() {
    stage.children = [];
    bullets = [];
    isOver = false;
    initGame();
    renderer.render(stage);

    ga("send", {
        hitType: "event",
        eventCategory: "game",
        eventAction: "play",
        eventLabel: userName
    });
}

function setGameNameTxet() {
    for (var i = 0; i < gameName.length; i++) {
        var txt = new PIXI.Text(gameName[i], { font: "bold " + canvasWidth * 0.1 + "px Arvo", fill: 0x3E1707, align: "center", stroke: 0xA4410E, strokeThickness: 3 });
        txt.name = "gameName" + i;
        if (i === 0) {
            var totalWidth = txt.width * gameName.length * 1.45;
            txt.x = (canvasWidth - totalWidth) / 2;
        }
        else {
            txt.x = stage.getChildByName("gameName" + (i - 1)).x + txt.width * 1.5;
        }

        txt.y = txt.height * -1;
        stage.addChild(txt);
    }
}

function gameStartAnimation() {
    if (stage.getChildByName("startBtn")) {
        return;
    }

    for (var i = 0; i < gameName.length; i++) {
        var txt = stage.getChildByName("gameName" + i);
        if (txt.y < 70) {
            txt.y += 10;
            break;
        } else {

            if (i === gameName.length - 1) {
                stage.addChild(getStartBtn(resources.start.texture));
                renderer.render(stage);
            }
        }
    }

    renderer.render(stage);
    requestAnimationFrame(gameStartAnimation);
}

function getBckshadow() {
    var shadow = new PIXI.Graphics();
    shadow.beginFill(0X2E2E2E, 0.5);
    shadow.drawRect(0, 0, canvasWidth, canvasHeight);
    shadow.endFill();

    return shadow;
}

function volumeswitch() {
    createjs.Sound.volume = createjs.Sound.volume > 0 ? 0 : 1;
    this.texture = getVolumeTexture();
}
PIXI.Texture.fromImage
function getVolumeTexture() {
    return createjs.Sound.volume == 0 ? resources.volumeoff.texture : resources.volumeup.texture;
}

function getStartBtn(btnTexture) {
    var startBtn = new PIXI.Sprite(btnTexture);
    startBtn.name = "startBtn";
    startBtn.width = canvasWidth * 0.2;
    startBtn.height = canvasHeight * 0.125;
    startBtn.buttonMode = true;
    startBtn.interactive = true;
    startBtn.position.x = (canvasWidth - startBtn.width) / 2;
    startBtn.position.y = canvasHeight * 0.725;
    startBtn.on("mousedown", startGame).on("touchstart", startGame);

    return startBtn;
}

function counter(second) {
    setTimeout(function () {
        second -= 1;
        if (stage.getChildByName("timer").text === "00:00") {
            gameover();
            var person = { score: stage.getChildByName("score").text };
            $.get("/games/ranks", person, function (response) {
                var ranks = "";
                var style = {
                    font: "bold italic " + canvasWidth * 0.035 + "px Arial",
                    fill: "#F7EDCA",
                    stroke: "#4a1850",
                    strokeThickness: 5,
                    wordWrap: true,
                    wordWrapWidth: 440
                };

                var ranks = [];
                var rankTxt = null;
                for (var i = 0; i < response.Persons.length; i++) {
                    var index = Math.floor(i / 5);
                    if (index > 1) {
                        break;
                    }
                    if (rankTxt == null || rankTxt.index != index) {
                        rankTxt = new PIXI.Text("", style);
                        rankTxt.y = canvasHeight * 0.2;
                        rankTxt.index = index;
                        ranks.push(rankTxt);
                    }
                    var person = response.Persons[i];
                    rankTxt.text += person.Rank + "  " + person.Name + "  " + person.Score + "\n";
                }

                if (ranks.length === 1) {
                    ranks[0].x = (canvasWidth - rankTxt.width) / 2;
                    stage.addChild(ranks[0]);
                } else {
                    var width = ranks[0].width + ranks[1].width + canvasWidth * 0.1;
                    ranks[0].x = (canvasWidth - width) / 2;
                    ranks[1].x = ranks[0].x + ranks[0].width + canvasWidth * 0.1;
                    stage.addChild(ranks[0]);
                    stage.addChild(ranks[1]);
                }

                var restartBtn = getStartBtn(resources.start.texture);
                stage.addChild(restartBtn);
                renderer.render(stage);
            });
        }

        if (second > -1) {
            var s = parseInt(second / 100);
            var ms = second % 100;
            stage.getChildByName("timer").text = (s < 10 ? "0" + s : s) + ':' + (ms < 10 ? "0" + ms : ms);
            counter(second);
        }
    }, 10);
}

function gameover() {
    isOver = true;
    bck._events.mousemove.fn = bck._events.mousedown.fn = bck._events.touchmove.fn = bck._events.touchend.fn = null;
    bullets = [];

    var style = {
        font: "bold italic " + canvasWidth * 0.1 + "px Arial",
        fill: "#F7EDCA",
        stroke: "#4a1850",
        strokeThickness: 5,
        dropShadow: true,
        dropShadowColor: "#000000",
        dropShadowAngle: Math.PI / 6,
        dropShadowDistance: 6,
        wordWrap: true,
        wordWrapWidth: canvasWidth * 1.25
    };
    var score = stage.getChildByName("score").text;
    var scoreTxt = new PIXI.Text("Your Score: " + score, style);
    scoreTxt.x = canvasWidth / 2 - scoreTxt.width / 2;

    stage.addChild(getBckshadow());
    stage.addChild(scoreTxt);

    renderer.render(stage);
    createjs.Sound.stop();
}

function initGame() {
    bck = new PIXI.Sprite(resources.backgroud.texture);
    bck.width = canvasWidth; bck.height = canvasHeight;
    bck.interactive = true;
    bck.on("mousemove", bckMoveEvent)
       .on("touchmove", bckMoveEvent)
       .on("mousedown", bckClickEvent)
       .on("touchend", bckClickEvent);
    stage.addChild(bck);

    var volumeTexture = getVolumeTexture();
    var volumeBtn = new PIXI.Sprite(volumeTexture);
    volumeBtn.width = canvasWidth * 0.06;
    volumeBtn.height = canvasHeight * 0.1;
    volumeBtn.position.y = canvasHeight - volumeBtn.height;
    volumeBtn.buttonMode = true;
    volumeBtn.interactive = true;
    volumeBtn.on("mousedown", volumeswitch).on("touchstart", volumeswitch);
    stage.addChild(volumeBtn);

    var scoreStyle = { font: "bold " + canvasWidth * 0.05 + "px Arial", fill: "yellow" }
    var score = new PIXI.Text("0", scoreStyle);
    score.name = "score";
    score.flag = false;
    score.x = canvasWidth - score.width;
    score.y = canvasHeight - score.height;
    stage.addChild(score);

    var gunBottom = new PIXI.Graphics();
    gunBottom.lineStyle(0);
    gunBottom.beginFill("#0x000000");
    gunBottom.drawCircle(canvasWidth / 2, canvasHeight, canvasWidth * 0.08);
    gunBottom.endFill();
    stage.addChild(gunBottom);

    var timerStyle = { font: "bold " + canvasWidth * 0.05 + "px Arial", fill: "white", align: "center" }
    var timer = new PIXI.Text("00:00", timerStyle);
    timer.name = "timer";
    timer.x = (canvasWidth / 2) - (timer.width / 2);
    timer.y = canvasHeight - timer.height;
    timer.text = "";
    stage.addChild(timer);

    target = new PIXI.Sprite(resources.pic.texture);
    target.width = target.height = canvasWidth * 0.1;
    stage.addChild(target);
    var player = null;
    createjs.Sound.on("fileload", function () {
        player = createjs.Sound.play("player");
        player.volume = 1;
    });

    if (!player && createjs.Sound.isReady()) {
        player = createjs.Sound.play("player");
    }

    renderer.render(stage);
    mainAnimation();
    counter(sec * 100);
}

function drawGun(position) {
    var gun = stage.getChildByName("gun");
    if (!gun) {
        gun = new PIXI.Graphics();
        gun.name = "gun";
    }
    gun.clear();
    gun.lineStyle(canvasWidth / 35, "#0x000000", 1);
    gun.moveTo(canvasWidth / 2, canvasHeight);
    gun.lineTo(position.Angle > 90 ? canvasWidth / 2 - position.X : canvasWidth / 2 + position.X, canvasHeight - position.Y);
    gun.endFill();
    stage.addChildAt(gun, 1);
    renderer.render(stage);
}

function getAngle(x1, y1, x2, y2) {
    var x = Math.abs(x1 - x2);
    var y = Math.abs(y1 - y2);
    var z = Math.sqrt(x * x + y * y);
    var angle = (Math.asin(y / z) * 180 / Math.PI);
    return x1 - x2 < 0 ? 180 - angle : angle;
}

function getXY(e) {
    var x = 0, y = 0;
    var position = {};
    if (e.data.originalEvent.type.indexOf("touch") > -1) {
        x = e.data.originalEvent.changedTouches[0].clientX;
        y = e.data.originalEvent.changedTouches[0].screenY - e.data.originalEvent.changedTouches[0].clientY;
    }
    else {
        x = e.data.originalEvent.offsetX;
        y = e.data.originalEvent.offsetY;
    }

    position.Angle = getAngle(x, y, canvasWidth / 2, canvasHeight);
    position.Y = Math.sin(Math.PI / 180 * position.Angle) * canvasWidth * 0.15;
    position.X = Math.sqrt(Math.pow(canvasWidth * 0.15, 2) - position.Y * position.Y);

    return position;
}

function bckMoveEvent(e) {
    if (e.data.originalEvent.target.tagName !== "CANVAS") {
        return;
    }
    var position = getXY(e);
    drawGun(position);
}

function bckClickEvent(e) {
    var position = getXY(e);

    if (e.data.originalEvent.type === "touchend") {
        drawGun(position);
    }

    var bullet = new PIXI.Graphics();
    bullet.angle = position.Angle;
    bullet.lineStyle(0);
    bullet.beginFill("0XFFFF00");
    bullet.drawCircle(position.Angle > 90 ? canvasWidth / 2 - position.X : canvasWidth / 2 + position.X, canvasHeight - position.Y, canvasWidth / 70);
    bullet.endFill();
    bullets.push(bullet);
    stage.addChild(bullet);

    createjs.Sound.play("bullet");
}

function bulletsAnimation() {
    if (bullets.length > 0) {
        for (var i = bullets.length - 1; i > -1; i--) {
            var bulletX = bullets[i].graphicsData[0].shape.x;
            var bulletY = bullets[i].graphicsData[0].shape.y;
            var pox = Math.cos(Math.PI / 180 * bullets[i].angle) * canvasHeight * 0.01;
            var poy = Math.sin(Math.PI / 180 * bullets[i].angle) * canvasHeight * 0.01 * (bullets[i].isBacklash ? -1 : 1);
            if (bullets[i].flag || bulletX < 0 || bulletX > canvasWidth || bulletY < 0 || bulletY > canvasHeight) {
                stage.removeChild(bullets[i]);
                bullets.splice(i, 1);
                continue;
            }

            if ((bulletY > 0 && bulletY < target.height) && (bulletX > target.x && bulletX < target.x + target.width)) {
                if (target.texture === resources.pic.texture && !bullets[i].flag) {
                    bullets[i].flag = true;
                    target.texture = resources.hit.texture;
                    target.interval = 0;
                    var score = stage.getChildByName("score");
                    score.flag = true;
                    score.text = parseInt(score.text) + 1;
                    score.position.x = canvasWidth - score.width;
                } else {
                    bullets[i].isBacklash = true;
                    createjs.Sound.play("backlash");
                }
            }

            bullets[i].x += pox;
            bullets[i].y -= poy;
            bullets[i].graphicsData[0].shape.x += pox;
            bullets[i].graphicsData[0].shape.y -= poy;
        }
    }
}

function mainAnimation() {
    if (isOver) {
        return;
    }

    if (target.position.x < 0) {
        shift = Math.abs(shift);
    }

    if (target.position.x > canvasWidth - target.width) {
        shift *= -1;
    }

    bulletsAnimation();

    var score = stage.getChildByName("score");
    if (score.flag && score.position.y < (canvasHeight - (score.height * 1.3))) {
        score.flag = false;
        score.position.y = canvasHeight - score.height;
    }

    if (score.flag) {
        score.position.y--;
    }

    if (target.texture === resources.hit.texture) {
        target.interval++;
        if (target.interval > 60) {
            target.texture = resources.pic.texture;
            target.interval = 0;
        }
    }

    target.x += shift;
    renderer.render(stage);
    requestAnimationFrame(mainAnimation);
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return "";
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}
