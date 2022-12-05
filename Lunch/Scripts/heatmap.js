function HeatMap(opts) {
    this.config(opts);
}

HeatMap.prototype = {
    //initialization
    config: function (opts) {
        var me = this;
        var w = opts.width, h = opts.height, id = opts.id;
        me.options = {
            width: w,
            height: h,
            // 透明度，取值範圍 0-255
            opacity: 150,
            // 半徑
            radius: 20,
            // 邊界模糊半徑
            bshadow: 1,
            boundVal: 15000,
            shadowBlur: 15,
            // 緩存畫布中點的數據，並記錄最大值
            points: {
                max: 20,
                data: []
            },
            // 调色板颜色取值范围
            gradient: { 0.45: "rgb(0,0,255)", 0.55: "rgb(0,255,255)", 0.65: "rgb(0,255,0)", 0.95: "yellow", 1.0: "rgb(255,0,0)" }
        };
        // heatmap 畫布
        var container = document.getElementById(id);
        var canvas = document.createElement("canvas"), ctx = canvas.getContext('2d');
        canvas.width = w;
        canvas.height = h;
        container.appendChild(canvas);

        // 調色板畫布
        var pcanvas = document.createElement("canvas"), pctx = pcanvas.getContext('2d');
        pcanvas.width = 1;
        pcanvas.height = 256;
        pcanvas.style.display = 'none';
        container.appendChild(pcanvas);

        me.options.ctx = ctx;
        me.options.pctx = pctx;
    },

    //繪製圓陰影：可視區域只繪製了一個圓陰影效果，而圓本身的位置在可視區域之外
    renderShadow: function (x, y, val, cache) {
        var me = this,
            ctx = me.options.ctx,
            radius = me.options.radius,
        	bval = me.options.boundVal;

        var opacity = parseFloat(val / me.options.points.max, 10);
        ctx.shadowColor = ('rgba(0, 0, 0, ' + opacity + ')');
        ctx.shadowOffsetX = bval;
        ctx.shadowOffsetY = bval;
        ctx.shadowBlur = me.options.shadowBlur;
        ctx.beginPath();
        ctx.arc(x - bval, y - bval, me.options.radius, 0, Math.PI * 2, true);
        ctx.closePath();
        ctx.fill();
    },

    // 繪製彩色熱圖根據調色板
    colorize: function () {
        var me = this, w = me.options.width, h = me.options.height, ctx = me.options.ctx;
        var img = ctx.getImageData(0, 0, w, h);
        var imgData = img.data, len = imgData.length, palette = me.getPalette();
        var opacity = me.options.opacity;

        for (var i = 3; i < len; i += 4) {
            // [0] -> r, [1] -> g, [2] -> b, [3] -> alpha
            var alpha = imgData[i],
			// 因為要取調色板中的顏色，而調色板數的顏色數據對應的步長是4
                offset = alpha * 4;

            if (!offset) {
                continue;
            }
            var finalAlpha = (alpha < opacity) ? alpha : opacity;
            imgData[i - 3] = palette[offset];
            imgData[i - 2] = palette[offset + 1];
            imgData[i - 1] = palette[offset + 2];
            imgData[i] = finalAlpha;
        }
        img.data = imgData;
        ctx.putImageData(img, 0, 0);
    },

    // 通過調色板來獲取平滑的顏色值
    getPalette: function () {
        var me = this, gradient = me.options.gradient, pctx = me.options.pctx;
        //緩存調色板數據
        var grad = me.options.pctx.createLinearGradient(0, 0, 1, 256);
        for (var x in gradient) {
            grad.addColorStop(x, gradient[x]);
        }
        pctx.fillStyle = grad;
        pctx.fillRect(0, 0, 1, 256);
        // 返回的是一個一位數組，每一個像素的的顏色有四個值來表示
        // 前三個值表示紅綠藍，第四個值表示alpha 通道
        // 也就是這個一位數組的長度是: 1 * 256 * 4;
        return pctx.getImageData(0, 0, 1, 256).data;
    },

    //get point data form db 
    getData: function (pathname) {
        var me = this;
        $.ajax({
            type: "POST",
            traditional: true,
            url: '/Heatmap/GetData',
            //dataType: "json",
            //contentType: 'application/json',
            data: { pathname: pathname },
            success: function (result) {
                if (result) {
                    var data = result, menu = [], body = [];
                    for (var i = 0; i < data.length; i++) {
                        var x = (data[i][0] * 20);
                        var y = (data[i][1] * 20);
                        if (y < document.getElementById("showMap").offsetTop) {
                            menu[menu.length] = [];
                            menu[menu.length - 1].push(x, y, data[i][2]);
                        }
                        else {
                            body[body.length] = [];
                            body[body.length - 1].push(x - document.getElementById("showMap").offsetLeft, y - document.getElementById("showMap").offsetTop, data[i][2]);
                        }
                    }
                    me.show(menu, body);
                }
            }
        });
    },

    //畫輪廓、上色
    displayHeatmap: function (data, div) {
        var me = this, tempData = []; tempData[2] = 1;
        for (var i = 0; i < data.length; i++) {
            me.renderShadow(data[i][0], data[i][1], data[i][2]);
            if (data[i][2] > tempData[2]) {
                tempData[0] = data[i][0];
                tempData[1] = data[i][1];
                tempData[2] = data[i][2];
            }
        }
        me.printData(tempData[0], tempData[1], tempData[2], div);
        me.colorize();
    },

    //show max value
    printData: function creatText(x, y, text, div) {
        var textDiv = document.createElement('div');
        textDiv.style.position = "absolute";
        textDiv.style.left = x + "px";
        textDiv.style.top = y + "px";
        textDiv.style.fontFamily = "Arial";
        textDiv.textContent = "max:" + text;
        document.getElementById(div).appendChild(textDiv);
    },
    //clean temp array
    cleanArray: function () {
        var data = this.options.points.data;
        data.length = 0;
    },

    //show time canvas in new page
    getTimeCount: function () {
        var me = this;
        $.ajax({
            type: "POST",
            traditional: true,
            url: '/Heatmap/TimeCount',
            data: { pathname: location.href },
            success: function (result) {
                //definition time chart
                var lineChartData = {
                    labels: ["0~3", "4~7", "8~11", "12~15", "16~19", "20~23"],
                    datasets: [
                        {
                            label: "My  dataset",
                            fillColor: "rgba(151,187,205,0.2)",
                            strokeColor: "rgba(151,187,205,1)",
                            pointColor: "rgba(151,187,205,1)",
                            pointStrokeColor: "#fff",
                            pointHighlightFill: "#fff",
                            pointHighlightStroke: "rgba(151,187,205,1)",
                            data: me.timeData(result)
                        }
                    ]
                }
                var timeCanvas = document.createElement('div');
                timeCanvas.style.width = '30%';
                timeCanvas.style.zIndex = 10000;
                var canvas = document.createElement("canvas"), ctx = canvas.getContext('2d');
                timeCanvas.appendChild(canvas);
                document.body.appendChild(timeCanvas);
                window.myLine = new Chart(ctx).Line(lineChartData, {
                    responsive: true
                })
                //pass time chart to newpage
                var myWindow = window.open('', '', 'width=650,height=350');
                var doc = myWindow.document;
                doc.open();
                doc.appendChild(timeCanvas);
                doc.close();
            },
            error: function (result) {
                alert(result);
            }
        });
    },

    //整理 time data
    timeData: function (tData) {
        var data = [0, 0, 0, 0, 0, 0];
        for (var i = 0; i < tData.length; i++) {
            var h = tData[i][0];
            var index = Math.floor((h - 4) / 4);
            data[index] = tData[i][1];
        }
        return data;
    },
}

//pass  temp array to controller
HeatMap.prototype.store = function () {
    var me = this, points = me.options.points, data = points.data;
    if (data.length != 0) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: '/Heatmap/StoreData',
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (data) {
                alert("OKOK");
            },
        });
    }
    me.cleanArray();
}

var heatmap = new HeatMap({
    width: $(document).width(),
    height: $(document).height(),
    id: 'heatmapContainer',
});
var heatmapMenu = new HeatMap({
    id: 'heatmapMenu',
});

// insert point
HeatMap.prototype.addPoint = function (x, y) {
    var me = this;
    me.options.ctx.clearRect(0, 0, me.options.width, me.options.height);
    me.options.pctx.clearRect(0, 0, 1, 256);
    var data = me.options.points.data, len = data.length, flag = false;
    for (var i = 0; i < len; i++) {
        if (data[i][0] == x && data[i][1] == y) {
            data[i][2] += 1;
            if (data[i][2] > me.options.points.max) {
                me.options.points.max = data[i][2];
            }
            flag = true;
        }
    }
    if (flag == false) {
        var date = new Date();
        data.push([x, y, 1, location.href.split('?')[0], date.toUTCString()]);
    }
    if (len > 200)
        me.store();
}

//creat show canvas
HeatMap.prototype.show = function (menu, body) {

    var showmap = new HeatMap({
        width: document.getElementById("heatmapContainer").clientWidth,
        height: document.getElementById("heatmapContainer").clientHeight,
        id: 'showMap',
        opacity: 100
    });
    showmap.displayHeatmap(body, "showMap");
    showmap.getTimeCount();
    if (menu != null) {
        var showMenu = new HeatMap({
            width: document.getElementById("heatmapMenu").clientWidth,
            height: document.getElementById("showMap").offsetTop,
            id: 'showMenu',
            opacity: 100
        });
        document.getElementById("showMenu").style.top = "0px";
        showMenu.displayHeatmap(menu, "showMenu");
    }
}
//refresh or leave to save data
window.onunload = function () {
    heatmap.store();
};

window.onload = function () {
    if (location.pathname != "/Heatmap/show") {
        var url = "";
        if (document.referrer != "")
            url = document.referrer.match(/\/\/.*?\/(.*?)\/?(\?.*)?$/)[1];

        if (url == "Heatmap/show") {
            heatmap.getData(location.href);
        }
        else {
            var data = [], container = document.getElementById('heatmapContainer'), menu = document.getElementById('heatmapMenu');
            //body click move
            container.onclick = container.onmousemove = container.ontouchmove = function (e) {
                var x = parseInt(e.pageX / 20, 10), y = parseInt((e.pageY - container.style.left) / 20, 10);
                heatmap.addPoint(x, y);
            };
            //menu click move
            menu.onclick = menu.onmousemove = menu.ontouchmove = function (e) {
                var x = parseInt(e.pageX / 20, 10), y = parseInt((e.pageY - document.body.scrollTop) / 20, 10);
                heatmap.addPoint(x, y);
            }

        }
    }
}

