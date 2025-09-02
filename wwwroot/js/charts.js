window.amc = (function () {
    const charts = {}; // id => root

    function dispose(id) {
        if (charts[id]) {
            charts[id].dispose();
            delete charts[id];
        }
    }

    function rootFor(id) {
        dispose(id);
        const root = am5.Root.new(id);
        root.setThemes([am5themes_Animated.new(root)]);
        charts[id] = root;
        return root;
    }

    function renderSparkline(id, values) {
        const root = rootFor(id);
        const chart = root.container.children.push(
            am5xy.XYChart.new(root, {
                panX: false, panY: false, wheelX: "none", wheelY: "none",
                paddingLeft: 0, paddingRight: 0, paddingTop: 0, paddingBottom: 0
            })
        );

        const xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
            categoryField: "i",
            renderer: am5xy.AxisRendererX.new(root, { minGridDistance: 10, inside: true })
        }));
        const yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
            renderer: am5xy.AxisRendererY.new(root, { inside: true })
        }));

        const series = chart.series.push(am5xy.LineSeries.new(root, {
            xAxis, yAxis, valueYField: "v", categoryXField: "i",
            tension: 0.9, strokeWidth: 2
        }));

        series.data.setAll(values.map((v, i) => ({ i: i.toString(), v })));
        xAxis.data.setAll(values.map((_, i) => ({ i: i.toString() })));

        series.fills.template.setAll({ visible: true, fillOpacity: 0.15 });
        series.bullets.push(() => am5.Circle.new(root, { radius: 2 }));
    }

    function renderHistogram(id, values) {
        const root = rootFor(id);
        const chart = root.container.children.push(am5xy.XYChart.new(root, { panX: false, panY: false }));
        const xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
            categoryField: "b",
            renderer: am5xy.AxisRendererX.new(root, { minGridDistance: 20 })
        }));
        const yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, { renderer: am5xy.AxisRendererY.new(root, {}) }));
        const series = chart.series.push(am5xy.ColumnSeries.new(root, {
            xAxis, yAxis, categoryXField: "b", valueYField: "v",
            clustered: false
        }));

        const data = values.map((v, i) => ({ b: `${i * 10}-${i * 10 + 9}%`, v }));
        series.data.setAll(data);
        xAxis.data.setAll(data);
    }

    function renderDonut(id, percent, label) {
        const root = rootFor(id);
        const chart = root.container.children.push(am5percent.PieChart.new(root, {
            innerRadius: am5.percent(65)
        }));
        const series = chart.series.push(am5percent.PieSeries.new(root, {
            valueField: "value", categoryField: "cat"
        }));

        series.data.setAll([
            { cat: "Done", value: Math.max(0, percent) },
            { cat: "Remaining", value: Math.max(0, 100 - percent) }
        ]);

        const center = am5.Label.new(root, {
            text: `${Math.round(percent)}%\n${label}`,
            textAlign: "center", centerX: am5.percent(50), centerY: am5.percent(50)
        });
        chart.seriesContainer.children.push(center);
    }

    // Semi-circle gauge using RadarChart
    function renderGauge(id, value, target) {
        const root = rootFor(id);

        const chart = root.container.children.push(am5radar.RadarChart.new(root, {
            startAngle: 180, endAngle: 360, innerRadius: am5.percent(40)
        }));

        const axisRenderer = am5radar.AxisRendererCircular.new(root, {});
        axisRenderer.grid.template.setAll({ visible: false });

        const valueAxis = chart.xAxes.push(am5xy.ValueAxis.new(root, {
            min: 0, max: 100, strictMinMax: true, renderer: axisRenderer
        }));

        // Ranges: red 0-50, yellow 50-85, green 85-100
        const ranges = [
            { from: 0, to: 50 },
            { from: 50, to: 85 },
            { from: 85, to: 100 }
        ];
        ranges.forEach((r, i) => {
            const range = valueAxis.createAxisRange(valueAxis.makeDataItem({ value: r.from, endValue: r.to }));
            range.get("axisFill").setAll({ visible: true, opacity: 0.7 });
            if (i === 0) range.get("axisFill").setAll({ fill: am5.color(0xff6b6b) });
            if (i === 1) range.get("axisFill").setAll({ fill: am5.color(0xffd166) });
            if (i === 2) range.get("axisFill").setAll({ fill: am5.color(0x06d6a0) });
        });

        const handDataItem = valueAxis.makeDataItem({ value });
        const hand = am5radar.ClockHand.new(root, { pinRadius: am5.percent(12), innerRadius: am5.percent(0) });
        handDataItem.set("bullet", am5xy.AxisBullet.new(root, { sprite: hand }));
        valueAxis.createAxisRange(handDataItem);

        const lbl = chart.children.push(am5.Label.new(root, {
            centerX: am5.percent(50), centerY: am5.percent(90),
            text: `${value.toFixed(1)}% (Target ${target}%)`
        }));
    }

    function renderSparklineTiny(id, values) {
        const root = rootFor(id);
        const chart = root.container.children.push(am5xy.XYChart.new(root, {
            paddingLeft: 0, paddingRight: 0, paddingTop: 0, paddingBottom: 0,
            panX: false, panY: false, wheelX: "none", wheelY: "none"
        }));
        const xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
            categoryField: "i",
            renderer: am5xy.AxisRendererX.new(root, { minGridDistance: 10, inside: true })
        }));
        const yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
            renderer: am5xy.AxisRendererY.new(root, { inside: true })
        }));
        const series = chart.series.push(am5xy.LineSeries.new(root, {
            xAxis, yAxis, valueYField: "v", categoryXField: "i", strokeWidth: 1.5, tension: 0.8
        }));
        const data = values.map((v, i) => ({ i: i.toString(), v }));
        series.data.setAll(data);
        xAxis.data.setAll(data);
        series.fills.template.setAll({ visible: true, fillOpacity: 0.1 });
    }

    return {
        dispose,
        renderSparkline,
        renderHistogram,
        renderDonut,
        renderGauge,
        renderSparklineTiny
    };
})();
