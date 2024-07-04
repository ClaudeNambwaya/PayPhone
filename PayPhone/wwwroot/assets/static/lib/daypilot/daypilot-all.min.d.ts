/* Copyright 2005 - 2021 Annpoint, s.r.o.
 Use of this software is subject to license terms.
 https://www.daypilot.org/
 */

type GlobalDate = Date;

export module DayPilot {

    class Scheduler extends SchedulerPropsAndEvents {
        v: string;
        events: {
            list: EventData[];

            add(e: DayPilot.Event): void;
            add(data: EventData): void;
            all(): DayPilot.Event[];
            edit(e: DayPilot.Event): void;
            filter(param: any): void;
            find(id: EventId): DayPilot.Event;
            find(filter: (data: DayPilot.Event) => boolean): DayPilot.Event;
            findAll(filter: (e: DayPilot.Event) => boolean): DayPilot.Event[];
            findAll(example: any): DayPilot.Event[];
            findRecurrent(masterId: string, time: DayPilot.Date | string): DayPilot.Event;
            focus(e: DayPilot.Event): void;
            forRange(start?: DayPilot.Date | string, end?: DayPilot.Date | string): DayPilot.Event[];
            load(url: string,
                 success: (args: { data: any; preventDefault(): void; }) => void,
                 error: (args: { request: XMLHttpRequest, exception: any; }) => void
            ): void;
            remove(e: DayPilot.Event): void;
            remove(data: EventData): void;
            remove(id: EventId): void;
            scrollIntoView(e: DayPilot.Event): void;
            update(e: DayPilot.Event): void;
            update(data: EventData): void;
        };
        cells: {
            all(): CellArray;
            findXy(x: number, y: number, grid?: string): CellArray;
            find(start: DayPilot.Date, resource: string): CellArray;
            findByPixels(x: number, y: number): CellArray;
        };
        infinite: {
            scrollTo(date: DayPilot.Date): void;
            shiftStart(days: number): void;
        };
        keyboard: {
            focusCell(date: DayPilot.Date, resource: ResourceId): void;
            focusEvent(e: DayPilot.Event): void;
            focusEvent(id: EventId): void;
            getFocus(): { e?: DayPilot.Event, cell?: {start: DayPilot.Date, end: DayPilot.Date, resource: ResourceId } };
            move(direction: "left" | "right" | "up" | "down", options?: { dontFireEvent?: boolean }): void;
            resetFocus(): void;
        };
        links: {
            add(link: DayPilot.Link): void;
            add(data: LinkData): void;
            find(id: LinkId): DayPilot.Link;
            findByFromTo(from: DayPilot.Date | string, to: DayPilot.Date | string): DayPilot.Link;
            remove(link: DayPilot.Link): void;
            remove(data: LinkData): void;
            remove(id: LinkId): void;
            load(url: string,
                 success: (args: { data: any; preventDefault(): void; }) => void,
                 error: (args: { request: XMLHttpRequest, exception: any; }) => void
            ): void;
            list: LinkData[];
        };
        multiselect: {
            add(e: DayPilot.Event, dontRedraw?: boolean): void;
            clear(dontRedraw?: boolean): void;
            events(): DayPilot.Event[];
            get(): DayPilot.Event[];
            isSelected(e: DayPilot.Event): boolean;
            redraw(): void;
            remove(e: DayPilot.Event, dontRedraw?: boolean): void;
            startRectangle(): void;
        };
        // legacy
        range: {
            all(): DayPilot.Selection[];
        };
        multirange: {
            add(sel: DayPilot.Selection): void;
            clear(): void;
            get(): DayPilot.Selection[];
        };
        rows: {
            add(data: ResourceData): void;
            addChild(parent: DayPilot.Row, data: ResourceData): void;
            addChild(parent: string, data: ResourceData): void;
            addChild(parent: number, data: ResourceData): void;
            addChild(parent: ResourceData, data: ResourceData): void;
            all(): DayPilot.Row[];
            collapseAll(): void;
            each(f: () => DayPilot.Row): void;
            edit(row: DayPilot.Row): void;
            expand(level?: number): void;
            expandAll(): void;
            filter(param: any): void;
            find(filter: (row: DayPilot.Row) => boolean, startIndex?: number): DayPilot.Row;
            find(id: ResourceId, start?: DayPilot.Date | string): DayPilot.Row;
            headerHide(): void;
            headerShow(): void;
            headerToggle(): void;
            load(url: string,
                 success: (args: { data: any; preventDefault(): void; }) => void,
                 error: (args: { request: XMLHttpRequest, exception: any; }) => void
            ): void;
            remove(row: DayPilot.Row): void;
            remove(id: ResourceId): void;
            sort(spec?: string | { field: string, order?: "asc" | "desc" }): void;
            update(row: DayPilot.Row | ResourceData): void;
            visible(): DayPilot.Row[];

            selection: {
                add(row: DayPilot.Row): void;
                clear(): void;
                get(): DayPilot.Row[];
                isSelected(row: DayPilot.Row): boolean;
                remove(row: Row): void;
            };
        };
        zoom: {
            setActive(index: number, position?: "left" | "middle" | "right"): void;
            setActive(id: string, position?: "left" | "middle" | "right"): void;
            active: number;
        };

        constructor(id: string | HTMLElement, options?: SchedulerConfig);

        autoRefreshPause(): void;

        autoRefreshStart(force: boolean): void;

        clearSelection(): void;

        dispose(): void;

        exportAs(format?: "svg" | "png" | "jpeg", options?: any): Export;

        getCoords(): { x: number, y: number, row: DayPilot.Row, cell: DayPilot.Cell, time: DayPilot.Date, event: DayPilot.Event, eventOffset: { x: number, y: number } };

        getDate(pixels: number, precise?: boolean, isEnd?: boolean): DayPilot.Date;

        getScrollX(): number;

        getScrollY(): number;

        getViewPort(): SchedulerViewPort;

        hide(): void;

        init(): void;

        loadingStart(): void;

        loadingStop(): void;

        message(msg: string, options?: { delay?: number, cssClass?: string, rawHtml?: boolean }): void;

        scrollTo(date: string | DayPilot.Date): void;

        scrollTo(date: string | DayPilot.Date, animated: boolean | number | "fast" | "normal" | "slow" | "linear", position?: "left" | "middle" | "right"): void;

        scrollToResource(id: ResourceId | DayPilot.Row): void;

        selectTimeRange(start: DayPilot.Date | string, end: DayPilot.Date | string, resource: ResourceId, dontFireEvent?: boolean): void;

        setHeight(pixels: number): void;

        setScroll(scrollX: number, scrollY: number): void;

        setScrollX(scrollX: number): void;

        setScrollY(scrollY: number): void;

        show(): void;

        uiBlock(): void;

        uiUnblock(): void;

        update(options?: SchedulerConfig): void;

        visibleStart(): DayPilot.Date;

        visibleEnd(): DayPilot.Date;
    }

    class SchedulerPropsAndEvents {
        allowDefaultContextMenu?: boolean;
        allowEventOverlap?: boolean;
        allowMultiMove?: boolean;
        allowMultiRange?: boolean;
        allowMultiResize?: boolean;
        allowMultiSelect?: boolean;
        api?: number;
        autoRefreshCommand?: string;
        autoRefreshEnabled?: boolean;
        autoRefreshInterval?: number;
        autoRefreshMaxCount?: number;
        autoScroll?: "Drag" | "Always" | "Disabled";
        backendUrl?: string;
        beforeCellRenderCaching?: boolean;
        blockOnCallBack?: boolean;
        bubble?: DayPilot.Bubble;
        businessBeginsHour?: number;
        businessEndsHour?: number;
        businessWeekends?: boolean;
        cellBubble?: DayPilot.Bubble;
        cellDuration?: number;
        cellGroupBy?: GroupBy;
        cellSweeping?: boolean;
        cellSweepingCacheSize?: number;
        cellWidth?: number;
        cellWidthMin?: number;
        cellWidthSpec?: "Auto" | "Fixed";
        cellsMarkBusiness?: boolean;
        clientState?: any;
        contextMenu?: DayPilot.Menu;
        contextMenuLink?: DayPilot.Menu;
        contextMenuResource?: DayPilot.Menu;
        contextMenuSelection?: DayPilot.Menu;
        cornerHtml?: string;
        cornerText?: string;
        crosshairTimeHeaderLevel?: "Last" | number;
        crosshairType?: "Full" | "Header" | "Disabled";
        days?: number;
        doubleClickTimeout?: number;
        dragOutAllowed?: boolean;
        drawBlankCells?: boolean;
        durationBarHeight?: number;
        durationBarMode?: "Duration" | "PercentComplete";
        durationBarVisible?: boolean;
        dynamicEventRendering?: "Progressive" | "Disabled";
        dynamicEventRenderingCacheSize?: number;
        dynamicEventRenderingCacheSweeping?: boolean;
        dynamicEventRenderingMargin?: number;
        dynamicEventRenderingMarginX?: number;
        dynamicEventRenderingMarginY?: number;
        dynamicLoading?: boolean;
        eventClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select" | "ContextMenu" | "Bubble";
        eventDeleteHandling?: "Update" | "Disabled" | "CallBack";
        eventDoubleClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select" | "ContextMenu" | "Bubble";
        eventEditHandling?: "Disabled" | "Update" | "CallBack";
        eventEditMinWidth?: number;
        eventEndSpec?: "DateTime" | "Date";
        eventHeight?: number;
        eventHoverHandling?: "Bubble" | "Disabled";
        eventHtmlLeftMargin?: number;
        eventHtmlRightMargin?: number;
        eventMarginBottom?: number;
        eventMarginLeft?: number;
        eventMarginRight?: number;
        eventMinWidth?: number;
        eventMoveHandling?: "Update" | "Disabled" | "CallBack" | "Notify";
        eventMoveMargin?: number;
        eventMoveSkipNonBusiness?: boolean;
        eventMoveToPosition?: boolean;
        eventMovingStartEndEnabled?: boolean;
        eventMovingStartEndFormat?: string;
        eventResizeHandling?: "Update" | "Disabled" | "CallBack" | "Notify";
        eventResizeMargin?: number;
        eventResizingStartEndEnabled?: boolean;
        eventResizingStartEndFormat?: string;
        eventRightClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select" | "ContextMenu" | "Bubble";
        eventSelectHandling?: "Disabled" | "Update" | "CallBack";
        eventStackingLineHeight?: number;
        eventTapAndHoldHandling?: "Move" | "ContextMenu";
        eventTextWrappingEnabled?: boolean;
        eventUpdateInplaceOptimization?: boolean;
        eventVersionHeight?: number;
        eventVersionMargin?: number;
        eventVersionPosition?: "Above" | "Below";
        eventVersionsEnabled?: boolean;
        eventVersionsReserveSpace?: boolean;
        eventsLoadMethod?: "GET" | "POST";
        floatingEvents?: boolean;
        floatingTimeHeaders?: boolean;
        groupBubble?: DayPilot.Bubble;
        groupConcurrentEvents?: boolean;
        groupConcurrentEventsLimit?: number;
        headerHeight?: number;
        height?: number;
        heightSpec?: "Auto" | "Max" | "Fixed" | "Parent100Pct" | "Max100Pct";
        hideBorderFor100PctHeight?: boolean;
        hideUntilInit?: boolean;
        infiniteScrollingEnabled?: boolean;
        infiniteScrollingMargin?: number;
        infiniteScrollingStepDays?: number;
        initEventEnabled?: boolean;
        jointEventsMove?: boolean;
        jointEventsResize?: boolean;
        keyboardEnabled?: boolean;
        keyboardTarget?: "document" | "component";
        layout?: "DivBased" | "TableBased";
        linkBottomMargin?: number;
        linkBubble?: DayPilot.Bubble;
        linkCreateHandling?: "Disabled" | "Update" | "CallBack" | "Notify";
        linkPointSize?: number;
        linksLoadMethod?: "GET" | "POST";
        loadingLabelText?: string;
        loadingLabelHtml?: string;
        loadingLabelVisible?: boolean;
        locale?: string | DayPilot.Locale;
        messageBarPosition?: "Top" | "Bottom";
        messageHideAfter?: number;
        messageHideOnMouseOut?: boolean;
        moveBy?: "Full" | "Top" | "Left";
        multiMoveVerticalMode?: "Disabled" | "Master" | "All";
        //multiSelectRectangle?: "Disabled" | "Free" | "Row";
        rectangleSelectHandling?: "Disabled" | "Enabled" | "EventSelect";
        rectangleSelectMode?: "Free" | "Row";
        navigatorBackSync?: string | DayPilot.Navigator;
        notifyCommit?: "Immediate" | "Queue";
        overrideWheelScrolling?: boolean;
        progressiveRowRendering?: boolean;
        progressiveRowRenderingPreload?: number;
        resourceBubble?: DayPilot.Bubble;
        resourceCollapseHandling?: "Enabled" | "CallBack";
        resourceExpandHandling?: "Enabled" | "CallBack";
        resources?: ResourceData[];
        rowClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select";
        rowCreateHandling?: "Disabled" | "Enabled" | "CallBack";
        rowCreateHeight?: number;
        rowCreateHtml?: string;
        rowCreateText?: string;
        rowDoubleClickHandling?: "Disabled" | "Enabled" | "CallBack" | "Select" | "Edit";
        rowDragHandleWidth?: number;
        rowEditHandling?: "Update" | "CallBack";
        rowFilterParentsAlwaysVisible?: boolean;
        rowHeaderColumnDefaultWidth?: number;
        rowHeaderColumnResizedHandling?: "Update" | "CallBack";
        rowHeaderColumns?: RowHeaderColumnData[];
        rowHeaderColumnsMode?: "Tabular" | "Legacy";
        rowHeaderColumnsResizable?: boolean;
        rowHeaderHideIconEnabled?: boolean;
        rowHeaderScrolling?: boolean;
        rowHeaderSplitterWidth?: number;
        rowHeaderWidth?: number;
        rowHeaderWidthAutoFit?: boolean;
        rowHeaderWidthMarginRight?: number;
        rowMarginBottom?: number;
        rowMarginTop?: number;
        rowMinHeight?: number;
        rowMoveHandling?: "Disabled" | "Update" | "CallBack" | "Notify";
        rowMoveSameLevelOnly?: boolean;
        rowRightClickHandling?: "ContextMenu" | "Enabled" | "Disabled";
        rowSelectHandling?: "Update" | "CallBack" | "Notify";
        rowsLoadMethod?: "GET" | "POST";
        rowSortingMode?: "LeavesOnly" | "ParentsOnly";
        scale?: "Manual" | "CellDuration" | "Minute" | "Hour" | "Day" | "Week" | "Month" | "Year";
        scrollDelayCells?: number;
        scrollDelayDynamic?: number;
        scrollDelayEvents?: number;
        scrollDelayFloats?: number;
        scrollDelayRows?: number;
        scrollStep?: number;
        scrollX?: number;
        scrollY?: number;
        selectedRows?: string[] | number[];
        separators?: SeparatorData[];
        showCurrentTimeHeader?: boolean;
        showNonBusiness?: boolean;
        showToolTip?: boolean;
        snapToGrid?: boolean;
        snapToGridEventMoving?: boolean;
        snapToGridEventResizing?: boolean;
        snapToGridRectangleSelecting?: boolean;
        snapToGridTimeRangeSelecting?: boolean;
        sortDirections?: SortDirection[];
        startDate?: DayPilot.Date | string;
        syncResourceTree?: boolean;
        tapAndHoldTimeout?: number;
        theme?: string;
        timeFormat?: "Auto" | "Clock12Hours" | "Clock24Hours";
        timeHeaderClickHandling?: "Enabled" | "Disabled";
        timeHeaderTextWrappingEnabled?: boolean;
        timeHeaders?: TimeHeaderData[];
        timeRangeClickHandling?: "Enabled" | "Disabled";
        timeRangeDoubleClickHandling?: "Disabled" | "CallBack" | "Enabled";
        timeRangeRightClickHandling?: "ContextMenu" | "Enabled" | "Disabled";
        timeRangeSelectedHandling?: "Enabled" | "Disabled" | "CallBack";
        timeRangeSelectingStartEndEnabled?: boolean;
        timeRangeSelectingStartEndFormat?: string;
        timeline?: TimelineData[];
        treeAnimation?: boolean;
        treeAutoExpand?: boolean;
        treeEnabled?: boolean;
        treeImageMarginLeft?: number;
        treeImageMarginTop?: number;
        treeImageWidth?: number;
        treeImageHeight?: number;
        treeIndent?: number;
        treePreventParentUsage?: boolean;
        useEventBoxes?: "Always" | "Never" | "ShortEventsOnly";
        viewType?: "Days" | "Resources" | "Gantt";
        visible?: boolean;
        watchWidthChanges?: boolean;
        weekStarts?: "Auto" | number;
        width?: number;
        xssProtection?: "Enabled" | "Disabled";
        zoomLevels?: ZoomLevel[];
        zoomPosition?: "left" | "right" | "middle";

        onAfterCellRender?: EventHandler<SchedulerAfterCellRenderArgs>;
        onAfterEventEditRender?: EventHandler<SchedulerAfterEventEditRenderArgs>;
        onAfterEventRender?: EventHandler<SchedulerAfterEventRenderArgs>;
        onAfterRender?: EventHandler<SchedulerAfterRenderArgs>;
        onAfterUpdate?: EventHandler<SchedulerAfterUpdateArgs>;
        onAutoRefresh?: EventHandler<SchedulerAutoRefreshArgs>;
        onBeforeCellExport?: EventHandler<SchedulerBeforeCellExportArgs>;
        onBeforeCellRender?: EventHandler<SchedulerBeforeCellRenderArgs>;
        onBeforeCornerExport?: EventHandler<SchedulerBeforeCornerExportArgs>;
        onBeforeCornerRender?: EventHandler<SchedulerBeforeCornerRenderArgs>;
        onBeforeCornerDomAdd?: EventHandler<SchedulerBeforeCornerDomAddArgs>;
        onBeforeCornerDomRemove?: EventHandler<SchedulerBeforeCornerDomRemoveArgs>;
        onBeforeEventExport?: EventHandler<SchedulerBeforeEventExportArgs>;
        onBeforeEventRender?: EventHandler<SchedulerBeforeEventRenderArgs>;
        onBeforeEventDomAdd?: EventHandler<SchedulerBeforeEventDomAddArgs>;
        onBeforeEventDomRemove?: EventHandler<SchedulerBeforeEventDomRemoveArgs>;
        onBeforeGridLineRender?: EventHandler<SchedulerBeforeGridLineRenderArgs>;
        onBeforeGroupRender?: EventHandler<SchedulerBeforeGroupRenderArgs>;
        onBeforeResHeaderRender?: EventHandler<SchedulerBeforeResHeaderRenderArgs>;
        onBeforeRowHeaderColumnRender?: EventHandler<SchedulerBeforeRowHeaderColumnRenderArgs>;
        onBeforeRowHeaderRender?: EventHandler<SchedulerBeforeRowHeaderRenderArgs>;
        onBeforeRowHeaderDomAdd?: EventHandler<SchedulerBeforeRowHeaderDomAddArgs>;
        onBeforeRowHeaderDomRemove?: EventHandler<SchedulerBeforeRowHeaderDomRemoveArgs>;
        onBeforeRowHeaderExport?: EventHandler<SchedulerBeforeRowHeaderExportArgs>;
        onBeforeTimeHeaderRender?: EventHandler<SchedulerBeforeTimeHeaderRenderArgs>;
        onBeforeTimeHeaderDomAdd?: EventHandler<SchedulerBeforeTimeHeaderDomAddArgs>;
        onBeforeTimeHeaderDomRemove?: EventHandler<SchedulerBeforeTimeHeaderDomRemoveArgs>;
        onBeforeTimeHeaderExport?: EventHandler<SchedulerBeforeTimeHeaderExportArgs>;
        onCallBackStart?: EventHandler<SchedulerCallBackStartArgs>;
        onCallBackEnd?: EventHandler<SchedulerCallBackEndArgs>;
        onCellMouseEnter?: EventHandler<SchedulerCellMouseEnterArgs>;
        onCellMouseLeave?: EventHandler<SchedulerCellMouseLeaveArgs>;
        /**
         * @deprecated This event has been replaced by onCellMouseLeave.
         */
        onCellMouseOut?: EventHandler<SchedulerCellMouseOutArgs>;
        /**
         * @deprecated This event has been replaced by onCellMouseEnter.
         */
        onCellMouseOver?: EventHandler<SchedulerCellMouseOverArgs>;
        onDimensionsChanged?: EventHandler<SchedulerDimensionsChangedArgs>;
        onEventClick?: EventHandler<SchedulerEventClickArgs>;
        onEventClicked?: EventHandler<SchedulerEventClickedArgs>;
        onEventDelete?: EventHandler<SchedulerEventDeleteArgs>;
        onEventDeleted?: EventHandler<SchedulerEventDeletedArgs>;
        onEventDoubleClick?: EventHandler<SchedulerEventDoubleClickArgs>;
        onEventDoubleClicked?: EventHandler<SchedulerEventDoubleClickedArgs>;
        onEventEdit?: EventHandler<SchedulerEventEditArgs>;
        onEventEdited?: EventHandler<SchedulerEventEditedArgs>;
        onEventEditKeyDown?: EventHandler<SchedulerEventEditKeyDownArgs>;
        onEventFilter?: EventHandler<SchedulerEventFilterArgs>;
        onEventMouseEnter?: EventHandler<SchedulerEventMouseEnterArgs>;
        onEventMouseLeave?: EventHandler<SchedulerEventMouseLeaveArgs>;
        /**
         * @deprecated This event has been replaced by onEventMouseLeave.
         */
        onEventMouseOut?: EventHandler<SchedulerEventMouseOutArgs>;
        /**
         * @deprecated This event has been replaced by onEventMouseEnter.
         */
        onEventMouseOver?: EventHandler<SchedulerEventMouseOverArgs>;
        onEventMove?: EventHandler<SchedulerEventMoveArgs>;
        onEventMoved?: EventHandler<SchedulerEventMovedArgs>;
        onEventMoving?: EventHandler<SchedulerEventMovingArgs>;
        onEventResize?: EventHandler<SchedulerEventResizeArgs>;
        onEventResized?: EventHandler<SchedulerEventResizedArgs>;
        onEventResizing?: EventHandler<SchedulerEventResizingArgs>;
        onEventRightClick?: EventHandler<SchedulerEventRightClickArgs>;
        onEventRightClicked?: EventHandler<SchedulerEventRightClickedArgs>;
        onEventSelect?: EventHandler<SchedulerEventSelectArgs>;
        onEventSelected?: EventHandler<SchedulerEventSelectedArgs>;
        onGridMouseDown?: EventHandler<SchedulerGridMouseDownArgs>;
        onIncludeTimeCell?: EventHandler<SchedulerIncludeTimeCellArgs>;
        onLinkClick?: EventHandler<SchedulerLinkClickArgs>;
        onLinkClicked?: EventHandler<SchedulerLinkClickedArgs>;
        onLinkCreate?: EventHandler<SchedulerLinkCreateArgs>;
        onLinkCreated?: EventHandler<SchedulerLinkCreatedArgs>;
        onLoadNode?: EventHandler<SchedulerLoadNodeArgs>;
        onRectangleSelect?: EventHandler<SchedulerRectangleSelectArgs>;
        onRectangleSelected?: EventHandler<SchedulerRectangleSelectedArgs>;
        onRectangleSelecting?: EventHandler<SchedulerRectangleSelectingArgs>;
        onResourceCollapse?: EventHandler<SchedulerResourceCollapseArgs>;
        onResourceExpand?: EventHandler<SchedulerResourceExpandArgs>;
        /**
         * @deprecated This event has been replaced by onRowClick.
         */
        onResourceHeaderClick?: EventHandler<SchedulerResourceHeaderClickArgs>;
        /**
         * @deprecated This event has been replaced by onRowClicked.
         */
        onResourceHeaderClicked?: EventHandler<SchedulerResourceHeaderClickedArgs>;
        onRowClick?: EventHandler<SchedulerRowClickArgs>;
        onRowClicked?: EventHandler<SchedulerRowClickedArgs>;
        onRowCreate?: EventHandler<SchedulerRowCreateArgs>;
        onRowCreated?: EventHandler<SchedulerRowCreatedArgs>;
        onRowDoubleClick?: EventHandler<SchedulerRowDoubleClickArgs>;
        onRowDoubleClicked?: EventHandler<SchedulerRowDoubleClickedArgs>;
        onRowEdit?: EventHandler<SchedulerRowEditArgs>;
        onRowEdited?: EventHandler<SchedulerRowEditedArgs>;
        onRowFilter?: EventHandler<SchedulerRowFilterArgs>;
        onRowHeaderColumnResized?: EventHandler<SchedulerRowHeaderColumnResizedArgs>;
        onRowHeaderResized?: EventHandler<SchedulerRowHeaderResizedArgs>;
        onRowMouseOver?: EventHandler<SchedulerRowMouseOverArgs>;
        onRowMouseOut?: EventHandler<SchedulerRowMouseOutArgs>;
        onRowMove?: EventHandler<SchedulerRowMoveArgs>;
        onRowMoved?: EventHandler<SchedulerRowMovedArgs>;
        onRowMoving?: EventHandler<SchedulerRowMovingArgs>;
        onRowSelect?: EventHandler<SchedulerRowSelectArgs>;
        onRowSelected?: EventHandler<SchedulerRowSelectedArgs>;
        onScroll?: EventHandler<SchedulerScrollArgs>;
        onTimeHeaderClick?: EventHandler<SchedulerTimeHeaderClickArgs>;
        onTimeHeaderClicked?: EventHandler<SchedulerTimeHeaderClickedArgs>;
        onTimeHeaderRightClick?: EventHandler<SchedulerTimeHeaderRightClickArgs>;
        onTimeHeaderRightClicked?: EventHandler<SchedulerTimeHeaderRightClickedArgs>;
        onTimeRangeClick?: EventHandler<SchedulerTimeRangeClickArgs>;
        onTimeRangeClicked?: EventHandler<SchedulerTimeRangeClickedArgs>;
        onTimeRangeDoubleClick?: EventHandler<SchedulerTimeRangeDoubleClickArgs>;
        onTimeRangeDoubleClicked?: EventHandler<SchedulerTimeRangeDoubleClickedArgs>;
        onTimeRangeRightClick?: EventHandler<SchedulerTimeRangeRightClickArgs>;
        onTimeRangeRightClicked?: EventHandler<SchedulerTimeRangeRightClickedArgs>;
        onTimeRangeSelect?: EventHandler<SchedulerTimeRangeSelectArgs>;
        onTimeRangeSelected?: EventHandler<SchedulerTimeRangeSelectedArgs>;
        onTimeRangeSelecting?: EventHandler<SchedulerTimeRangeSelectingArgs>;
    }

    class SchedulerConfig extends SchedulerPropsAndEvents {
        events?: EventData[];
        links?: LinkData[];
        zoom?: number | string;
    }

    namespace Scheduler {
        function makeDraggable(options: SchedulerMakeDraggableOptions): void;
        function registerDropTarget(options: SchedulerRegisterDropTargetOptions): void;
        function startDragging(options: any): void;
        function stopDragging(): void;
    }

    interface SchedulerRegisterDropTargetOptions {
        element: HTMLElement;
        onDrop?: EventHandler<SchedulerDropTargetDropArgs>;
        onDragOver?: EventHandler<SchedulerDropTargetDragOverArgs>;
        onDragLeave?: EventHandler<SchedulerDropTargetDragLeaveArgs>;
    }

    interface SchedulerMakeDraggableOptions {
        element: HTMLElement;
        keepElement?: boolean;
        remove?: HTMLElement;
        data?: SchedulerMakeDraggableData;
        id?: EventId;
        text?: string;
        duration?: number | DayPilot.Duration;
        externalHtml?: string;
        externalCssClass?: string;
    }

    interface SchedulerMakeDraggableData {
        id: EventId;
        text: string;
        duration: number | DayPilot.Duration;
    }

    interface SchedulerDropTargetDropArgs {
        readonly e: DayPilot.Event;
    }

    interface SchedulerDropTargetDragOverArgs {
        readonly e: DayPilot.Event;
    }

    interface SchedulerDropTargetDragLeaveArgs {
        readonly e: DayPilot.Event;
    }

    interface SchedulerAfterCellRenderArgs {
        readonly cell: {
            start: DayPilot.Date;
            end: DayPilot.Date;
            resource: ResourceId;
            x: number;
            y: number;
            displayY: number;
            grid: GridId;
        };
        readonly div: HTMLElement;
    }

    interface SchedulerAfterEventEditRenderArgs {
        readonly e: DayPilot.Event;
        readonly element: HTMLElement;
    }

    interface SchedulerAfterEventRenderArgs {
        readonly e: DayPilot.Event;
        readonly div: HTMLElement;
    }

    interface SchedulerAfterRenderArgs {
        readonly isCallBack: boolean;
        readonly isScroll: boolean;
        readonly data: any;
    }

    interface SchedulerAfterUpdateArgs {
    }

    interface SchedulerAutoRefreshArgs {
        readonly i: number;
        preventDefault(): void;
    }

    interface SchedulerBeforeCellExportArgs {
        readonly cell: DayPilot.Cell;
        text: string;
        horizontalAlignment: HorizontalAlignment;
        backColor: string;
    }

    interface SchedulerBeforeCellRenderArgs {
        readonly control: Scheduler;
        readonly cell: Cell;

        getPixels(date: DayPilot.Date | "string"): number;
    }

    interface SchedulerBeforeCornerExportArgs {
        backColor: string;
        text: string;
        fontSize: string;
        fontFamily: string;
        fontStyle: FontStyle;
        fontColor: string;
        horizontalAlignment: HorizontalAlignment;
        verticalAlignment: VerticalAlignment;
    }

    interface SchedulerBeforeCornerRenderArgs {
        readonly control: Scheduler;
        html: string;
        areas: AreaData[];
    }

    interface SchedulerBeforeCornerDomAddArgs {
        readonly control: Scheduler;
        element: any;  // HTMLElement or React component
    }

    interface SchedulerBeforeCornerDomRemoveArgs {
        readonly control: Scheduler;
        readonly element: any;  // HTMLElement or React component
    }

    interface SchedulerBeforeEventExportArgs {
        readonly e: DayPilot.Event;
        areas: AreaData[];
        fontSize: string;
        fontFamily: string;
        fontColor: string;
        backColor: string;
        borderColor: string;
        horizontalAlignment: HorizontalAlignment;
        verticalAlignment: VerticalAlignment;
        barHeight: number;
        textPadding: number;
        textLeft: string;
        textRight: string;
    }

    interface SchedulerBeforeEventRenderArgs {
        readonly data: EventData;
    }

    interface SchedulerBeforeEventDomAddArgs {
        readonly control: Scheduler;
        readonly e: DayPilot.Event;
        element: any;
    }

    interface SchedulerBeforeEventDomRemoveArgs {
        readonly control: Scheduler;
        readonly e: DayPilot.Event;
        readonly element: any;
    }

    interface SchedulerBeforeGridLineRenderArgs {
        readonly start?: DayPilot.Date;
        readonly end?: DayPilot.Date;
        readonly row?: DayPilot.Row;
        hidden: boolean;
        cssClass: string;
        readonly type: "HorizontalLine" | "VerticalLine" | "VerticalBreak";
    }

    interface SchedulerBeforeGroupRenderArgs {
        readonly group: RenderGroup;
    }

    interface SchedulerBeforeResHeaderRenderArgs {
        readonly resource: ResourceData;
    }

    interface SchedulerBeforeRowHeaderColumnRenderArgs {
        readonly column: {
            areas: AreaData[];
            cssClass: string;
            html: string;
            sortingEnabled: boolean;
            readonly data: RowHeaderColumnData;
        };
    }

    interface SchedulerBeforeRowHeaderRenderArgs {
        readonly row: RenderRow;
    }

    interface SchedulerBeforeRowHeaderDomAddArgs {
        readonly control: DayPilot.Scheduler;
        readonly row: DayPilot.Row;
        element: any;
        target: "Cell" | "Text";
    }

    interface SchedulerBeforeRowHeaderDomRemoveArgs {
        readonly control: DayPilot.Scheduler;
        readonly row: DayPilot.Row;
        readonly element: any;
        readonly target: "Cell" | "Text";
    }

    interface SchedulerBeforeRowHeaderExportArgs {
        readonly row: DayPilot.Row;
        text: string;
        backColor: string;
        borderColor: string;
        fontSize: string;
        fontFamily: string;
        fontStyle: FontStyle;
        fontColor: string;
        horizontalAlignment: HorizontalAlignment;
        verticalAlignment: VerticalAlignment;
        columns: RowHeaderExportColumn[];
    }

    interface SchedulerBeforeTimeHeaderRenderArgs {
        readonly control: Scheduler;
        readonly header: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly level: number;
            text: string;
            html: string;
            toolTip: string;
            backColor: string;
            fontColor: string;
            cssClass: string;
        };
    }

    interface SchedulerBeforeTimeHeaderDomAddArgs {
        readonly control: Scheduler;
        readonly header: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly level: number;
        };
        element: any;
    }

    interface SchedulerBeforeTimeHeaderDomRemoveArgs {
        readonly control: Scheduler;
        readonly header: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly level: number;
        };
        readonly element: any;
    }

    interface SchedulerBeforeTimeHeaderExportArgs {
        readonly control: Scheduler;
        readonly header: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly level: number;
            readonly text: string;
            readonly html: string;
        };
        text: string;
        backColor: string;
        fontColor: string;
        fontSize: string;
        fontFamily: string;
        fontStyle: FontStyle;
    }

    interface SchedulerCallBackStartArgs {
    }

    interface SchedulerCallBackEndArgs {
    }

    interface SchedulerCellMouseEnterArgs {
        readonly cell: DayPilot.Cell;
    }

    interface SchedulerCellMouseLeaveArgs {
        readonly cell: DayPilot.Cell;
    }

    interface SchedulerCellMouseOutArgs {
        [x: string]: any;
    }

    interface SchedulerCellMouseOverArgs {
        [x: string]: any;
    }

    interface SchedulerDimensionsChangedArgs {
        oldHeight: number;
        oldRowHeaderWidth: number;
        oldWidth: number;
        newHeight: number;
        newRowHeaderWidth: number;
        newWidth: number;
    }

    interface SchedulerEventClickArgs {
        readonly e: DayPilot.Event;
        readonly div: HTMLElement;
        readonly ctrl: boolean;
        readonly meta: boolean;
        readonly shift: boolean;
        readonly control: DayPilot.Scheduler;
        readonly originalEvent: MouseEvent;
        preventDefault(): void;
    }

    interface SchedulerEventClickedArgs {
        readonly e: DayPilot.Event;
        readonly div: HTMLElement;
        readonly ctrl: boolean;
        readonly meta: boolean;
        readonly shift: boolean;
        readonly control: DayPilot.Scheduler;
        readonly originalEvent: MouseEvent;
    }

    interface SchedulerEventDeleteArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Scheduler;
        preventDefault(): void;
    }

    interface SchedulerEventDeletedArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Scheduler;
    }

    interface SchedulerEventDoubleClickArgs {
        readonly e: DayPilot.Event;
        readonly originalEvent: MouseEvent;
        preventDefault(): void;
    }

    interface SchedulerEventDoubleClickedArgs {
        readonly e: DayPilot.Event;
        readonly originalEvent: MouseEvent;
    }

    interface SchedulerEventEditArgs {
        readonly e: DayPilot.Event;
        readonly canceled: boolean;
        readonly control: DayPilot.Scheduler;
        newText: string;
        async: boolean;
        loaded(): void;
        preventDefault(): void;
    }

    interface SchedulerEventEditedArgs {
        readonly e: DayPilot.Event;
        readonly canceled: boolean;
        readonly control: DayPilot.Scheduler;
        readonly newText: string;
    }

    interface SchedulerEventEditKeyDownArgs {
        readonly e: DayPilot.Event;
        readonly originalEvent: KeyboardEvent;
        cancel(): void;
        submit(): void;
        preventDefault(): void;
    }

    interface SchedulerEventFilterArgs {
        readonly e: DayPilot.Event;
        readonly filterParam: any;
        visible: boolean;
    }

    interface SchedulerEventMouseEnterArgs {
        readonly e: DayPilot.Event;
        readonly div: Element;
    }

    interface SchedulerEventMouseLeaveArgs {
        readonly e: DayPilot.Event;
        readonly div: HTMLElement;
    }

    interface SchedulerEventMouseOutArgs {
        [x: string]: any;
    }

    interface SchedulerEventMouseOverArgs {
        [x: string]: any;
    }

    interface SchedulerEventMoveArgs {
        async: boolean;
        readonly areaData: any;
        readonly control: DayPilot.Scheduler;
        readonly e: DayPilot.Event;
        readonly multimove: {
            readonly event: DayPilot.Event;
            start: DayPilot.Date;
            end: DayPilot.Date;
            resource: ResourceId;
            readonly overlapping: boolean;
        }[];
        newStart: DayPilot.Date;
        newEnd: DayPilot.Date;
        newResource: ResourceId;
        readonly external: boolean;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly position: number;

        loaded(): void;
        preventDefault(): void;
    }

    interface SchedulerEventMovedArgs {
        readonly async: boolean;
        readonly areaData: any;
        readonly control: DayPilot.Scheduler;
        readonly e: DayPilot.Event;
        readonly multimove: {
            readonly event: DayPilot.Event;
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly resource: ResourceId;
            readonly overlapping: boolean;
        }[];
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
        readonly newResource: ResourceId;
        readonly external: boolean;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly position: number;
    }

    interface SchedulerEventMovingArgs {
        allowed: boolean;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly alt: boolean;
        cssClass: string;
        readonly areaData: any;
        link: {
            from: DayPilot.Event;
            type?: LinkType;
            width?: number;
            color?: string;
            style?: string;
            cssClass?: string;
            layer?: "Above" | "Below";
        };
        start: DayPilot.Date;
        end: DayPilot.Date;
        readonly duration: DayPilot.Duration;
        readonly e: DayPilot.Event;
        readonly external: boolean;
        html: string;
        readonly resource: string;
        readonly row: DayPilot.Row;
        readonly position: number;
        left: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
        right: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
    }

    interface SchedulerEventResizeArgs {
        async: boolean;
        readonly control: DayPilot.Scheduler;
        readonly e: DayPilot.Event;
        readonly multiresize: {
            event: DayPilot.Event;
            start: DayPilot.Date;
            end: DayPilot.Date;
        }[];
        newStart: DayPilot.Date;
        newEnd: DayPilot.Date;
        readonly what: "start" | "end";

        loaded(): void;
        preventDefault(): void;
    }

    interface SchedulerEventResizedArgs {
        readonly async: boolean;
        readonly control: DayPilot.Scheduler;
        readonly e: DayPilot.Event;
        readonly multiresize: {
            readonly event: DayPilot.Event;
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
        }[];
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
        readonly what: "start" | "end";
    }

    interface SchedulerEventResizingArgs {
        start: DayPilot.Date;
        end: DayPilot.Date;
        readonly duration: DayPilot.Duration;
        readonly row: DayPilot.Row;
        readonly e: DayPilot.Event;
        allowed: boolean;
        readonly what: "start" | "end";
        readonly anchor: DayPilot.Date;
        readonly left: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
        readonly right: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
        cssClass: string;
        html: string;
        readonly overlapping: boolean;
        readonly shift: boolean;
        readonly ctrl: boolean;
        readonly alt: boolean;
        readonly meta: boolean;
        readonly multiresize: {
            event: DayPilot.Event;
            start: DayPilot.Date;
            end: DayPilot.Date;
        }[];
    }

    interface SchedulerEventRightClickArgs {
        readonly e: DayPilot.Event;
        readonly div: HTMLElement;
        readonly originalEvent: MouseEvent;

        preventDefault(): void;
    }

    interface SchedulerEventRightClickedArgs {
        readonly e: DayPilot.Event;
        readonly div: HTMLElement;
        readonly originalEvent: MouseEvent;
    }

    interface SchedulerEventSelectArgs {
        readonly e: DayPilot.Event;
        readonly selected: boolean;
        readonly ctrl: boolean;
        readonly meta: boolean;
        preventDefault(): void;
    }

    interface SchedulerEventSelectedArgs {
        readonly e: DayPilot.Event;
        readonly selected: boolean;
        readonly ctrl: boolean;
        readonly meta: boolean;
    }

    interface SchedulerGridMouseDownArgs {
        action: "None" | "RectangleSelect" | "TimeRangeSelect";
        readonly shift: boolean;
        readonly ctrl: boolean;
        readonly meta: boolean;
        readonly originalEvent: MouseEvent;
        readonly button: MouseButton;

        preventDefault(): void;
    }

    interface SchedulerIncludeTimeCellArgs {
        readonly cell: {
            start: DayPilot.Date;
            end: DayPilot.Date;
            visible: boolean;
            width: number;
        };
    }

    interface SchedulerLinkClickArgs {
        readonly link: LinkData;
    }

    interface SchedulerLinkClickedArgs {
        readonly link: LinkData;
    }

    interface SchedulerLinkCreateArgs {
        from: EventId;
        to: EventId;
        type: LinkType;
        id: LinkId;
        preventDefault(): void;
    }

    interface SchedulerLinkCreatedArgs {
        readonly from: EventId;
        readonly to: EventId;
        readonly type: LinkType;
        readonly id: LinkId;
    }

    interface SchedulerLoadNodeArgs {
        readonly resource: ResourceData;
        async: boolean;
        loaded(): void;
    }

    interface SchedulerRectangleSelectArgs {
        readonly events: DayPilot.Event[];
        append: boolean;
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resources: ResourceId[];
        preventDefault(): void;
    }

    interface SchedulerRectangleSelectedArgs {
        readonly events: DayPilot.Event[];
    }

    interface SchedulerRectangleSelectingArgs {
        readonly events: DayPilot.Event[];
        visible: boolean;
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resources: ResourceId[];
    }

    interface SchedulerResourceCollapseArgs {
        readonly resource: {
            readonly name: string;
            readonly id: ResourceId;
            readonly start: DayPilot.Date;
        };
        preventDefault(): void;
    }

    interface SchedulerResourceExpandArgs {
        readonly resource: {
            readonly name: string;
            readonly id: ResourceId;
            readonly start: DayPilot.Date;
        };
        preventDefault(): void;
    }

    interface SchedulerResourceHeaderClickArgs {
        [x: string]: any;
    }

    interface SchedulerResourceHeaderClickedArgs {
        [x: string]: any;
    }

    interface SchedulerRowClickArgs {
        readonly row: DayPilot.Row;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        preventDefault(): void;
    }

    interface SchedulerRowClickedArgs {
        readonly row: DayPilot.Row;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
    }

    interface SchedulerRowCreateArgs {
        text: string;
        preventDefault(): void;
    }

    interface SchedulerRowCreatedArgs {
        readonly text: string;
    }

    interface SchedulerRowDoubleClickArgs {
        readonly row: DayPilot.Row;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly originalEvent: MouseEvent;
        preventDefault(): void;
    }

    interface SchedulerRowDoubleClickedArgs {
        readonly row: DayPilot.Row;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly originalEvent: MouseEvent;
    }

    interface SchedulerRowEditArgs {
        async: boolean;
        readonly canceled: boolean;
        readonly newText: string;
        readonly row: DayPilot.Row;

        loaded(): void;
        preventDefault(): void;
    }

    interface SchedulerRowEditedArgs {
        readonly async: boolean;
        readonly canceled: boolean;
        readonly newText: string;
        readonly row: DayPilot.Row;
    }

    interface SchedulerRowFilterArgs {
        visible: boolean;
        readonly row: DayPilot.Row;
        readonly filterParam: any;
        readonly control: DayPilot.Scheduler;
    }

    interface SchedulerRowHeaderColumnResizedArgs {
        readonly column: RowHeaderColumnData;
    }

    interface SchedulerRowHeaderResizedArgs {
    }

    interface SchedulerRowMouseOverArgs {
        readonly div: HTMLElement;
        readonly row: DayPilot.Row;
    }

    interface SchedulerRowMouseOutArgs {
        readonly div: HTMLElement;
        readonly row: DayPilot.Row;
    }

    interface SchedulerRowMoveArgs {
        readonly source: DayPilot.Row;
        readonly target: DayPilot.Row;
        position: "child" | "before" | "after" | "forbidden";

        preventDefault(): void;
    }

    interface SchedulerRowMovedArgs {
        readonly source: DayPilot.Row;
        readonly target: DayPilot.Row;
        readonly position: "child" | "before" | "after" | "forbidden";
    }

    interface SchedulerRowMovingArgs {
        readonly source: DayPilot.Row;
        readonly target: DayPilot.Row;
        position: "child" | "before" | "after" | "forbidden";
    }

    interface SchedulerRowSelectArgs {
        readonly row: DayPilot.Row;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly selected: boolean;
        preventDefault(): void;
    }

    interface SchedulerRowSelectedArgs {
        readonly row: DayPilot.Row;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly selected: boolean;
    }

    interface SchedulerScrollArgs {
        readonly viewport: SchedulerViewPort;
        async: boolean;
        events: EventData[];
        remove: EventId[];
        clearEvents: boolean;
        dontForceCellRendering: boolean;
        loaded(): void;
    }

    interface SchedulerTimeHeaderClickArgs {
        readonly control: Scheduler;
        readonly header: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly level: number;
        };

        preventDefault(): void;
    }

    interface SchedulerTimeHeaderClickedArgs {
        readonly control: Scheduler;
        readonly header: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly level: number;
        };
    }

    interface SchedulerTimeHeaderRightClickArgs {
        readonly header: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly level: number;
        };

        preventDefault(): void;
    }

    interface SchedulerTimeHeaderRightClickedArgs {
        readonly header: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly level: number;
        };
    }

    interface SchedulerTimeRangeClickArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;

        preventDefault(): void;
    }

    interface SchedulerTimeRangeClickedArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
    }

    interface SchedulerTimeRangeDoubleClickArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;

        preventDefault(): void;
    }

    interface SchedulerTimeRangeDoubleClickedArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
    }

    interface SchedulerTimeRangeRightClickArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;

        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;

        preventDefault(): void;
    }

    interface SchedulerTimeRangeRightClickedArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;

        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
    }

    interface SchedulerTimeRangeSelectArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
        readonly control: DayPilot.Scheduler;
        readonly multirange: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly resource: ResourceId;
        };

        preventDefault(): void;
    }

    interface SchedulerTimeRangeSelectedArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
        readonly control: DayPilot.Scheduler;
        readonly multirange: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly resource: ResourceId;
        };
    }

    interface SchedulerTimeRangeSelectingArgs {
        readonly anchor: DayPilot.Date;
        start: DayPilot.Date;
        end: DayPilot.Date;
        readonly duration: DayPilot.Duration;
        readonly resource: ResourceId;
        ignoreDisabledCells: boolean;
        readonly row: DayPilot.Row;
        allowed: boolean;
        readonly left: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
        readonly right: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
        readonly overlapping: boolean;
        html: string;
        cssClass: string;
        button: MouseButton;
    }

    class Calendar extends CalendarPropsAndEvents {
        v: string;
        columns: {
            list: CalendarColumnData[];
            load(url: string,
                 success: (args: { data: any; preventDefault(): void; }) => void,
                 error: (args: { request: XMLHttpRequest, exception: any; }) => void
            ): void;
            filter(param: any): void;
        };
        events: {
            list: EventData[];
            add(e: DayPilot.Event | EventData): void;
            find(id: string): DayPilot.Event;
            findRecurrent(masterId: string, time: DayPilot.Date | string): DayPilot.Event;
            load(url: string,
                 success: (args: { data: any; preventDefault(): void; }) => void,
                 error: (args: { request: XMLHttpRequest, exception: any; }) => void
            ): void;
            remove(e: DayPilot.Event): void;
            remove(id: EventId): void;
            update(e: DayPilot.Event | EventData): void;
        };
        multiselect: {
            add(e: DayPilot.Event, dontRedraw?: boolean): void;
            clear(dontRedraw?: boolean): void;
            events(): DayPilot.Event[];
            redraw(): void;
            remove(e: DayPilot.Event, dontRedraw?: boolean): void;
        };

        constructor(id: string | HTMLElement, options?: CalendarConfig);

        clearSelection(): void;

        dispose(): void;

        exportAs(format?: "svg" | "png" | "jpeg", options?: any): Export;

        getSelection(): DayPilot.Selection;

        hide(): void;

        init(): void;

        message(msg: string, options?: { delay?: number, cssClass?: string, rawHtml?: boolean }): void;

        scrollToHour(hour: number): void;

        show(): void;

        update(options?: CalendarConfig): void;

        visibleStart(): DayPilot.Date;

        visibleEnd(): DayPilot.Date;

    }

    class CalendarPropsAndEvents {
        allDayEnd?: "DateTime" | "Date";
        allDayEventHeight?: number;
        allowEventOverlap?: boolean;
        allowMultiSelect?: boolean;
        api?: number;
        autoRefreshCommand?: string;
        autoRefreshEnabled?: boolean;
        autoRefreshInterval?: number;
        autoRefreshMaxCount?: number;
        autoScroll?: "Drag" | "Always" | "Disabled";
        backendUrl?: string;
        bubble?: DayPilot.Bubble;
        businessBeginsHour?: number;
        businessEndsHour?: number;
        cellDuration?: number;
        cellHeight?: number;
        clientState?: any;
        columnBubble?: DayPilot.Bubble;
        columnMarginRight?: number;
        columnsLoadMethod?: "POST" | "GET";
        columnWidth?: number;
        columnWidthSpec?: "Auto" | "Fixed";
        contextMenu?: DayPilot.Menu;
        contextMenuSelection?: DayPilot.Menu;
        cornerHtml?: string;
        cornerText?: string;
        crosshairType?: "Header" | "Full";
        dayBeginsHour?: number;
        dayEndsHour?: number;
        days?: number;
        doubleClickTimeout?: number;
        durationBarVisible?: boolean;
        durationBarWidth?: number;
        dynamicEventRendering?: "Progressive" | "Disabled";
        dynamicEventRenderingMarginX?: number;
        dynamicEventRenderingMarginY?: number;
        eventArrangement?: "SideBySide" | "Cascade" | "Full";
        eventClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select" | "ContextMenu" | "Bubble";
        eventDeleteHandling?: "Update" | "Disabled" | "CallBack";
        eventDoubleClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select" | "Bubble";
        eventEditHandling?: "Update" | "CallBack";
        eventHoverHandling?: "Bubble" | "Disabled";
        eventMoveHandling?: "Update" | "CallBack" | "Notify" | "Disabled";
        eventResizeHandling?: "Update" | "CallBack" | "Notify" | "Disabled";
        eventRightClickHandling?: "ContextMenu" | "Disabled" | "CallBack" | "Bubble";
        eventSelectHandling?: "Update" | "CallBack" | "Disabled";
        eventTapAndHoldHandling?: "Move" | "ContextMenu";
        headerClickHandling?: "Enabled" | "CallBack";
        headerDateFormat?: string;
        headerHeight?: number;
        headerLevels?: number;
        height?: number;
        heightSpec?: "BusinessHours" | "BusinessHoursNoScroll" | "Fixed" | "Full" | "Parent100Pct";
        hideFreeCells?: boolean;
        hideUntilInit?: boolean;
        hourWidth?: number;
        initScrollPos?: number;
        loadingLabelText?: string;
        loadingLabelHtml?: string;
        loadingLabelVisible?: boolean;
        locale?: string;
        messageHideAfter?: number;
        moveBy?: "Full" | "Left" | "Top" | "Disabled" | "None";
        notifyCommit?: "Immediate" | "Queue";
        rtl?: boolean;
        scrollDelayCells?: number;
        scrollDelayEvents?: number;
        showAllDayEvents?: boolean;
        showAllDayEventStartEnd?: boolean;
        showCurrentTime?: boolean;
        showCurrentTimeMode?: "Day" | "Full";
        showCurrentTimeOffset?: number;
        showHeader?: boolean;
        showHours?: boolean;
        showToolTip?: boolean;
        sortDirections?: SortDirection[];
        startDate?: DayPilot.Date | string;
        theme?: string;
        timeFormat?: "Auto" | "Clock12Hours" | "Clock24Hours";
        timeHeaderCellDuration?: number;
        timeRangeDoubleClickHandling?: "Enabled" | "Disabled" | "CallBack";
        timeRangeSelectedHandling?: "Enabled" | "Disabled" | "CallBack";
        useEventBoxes?: "Always" | "Never" | "ShortEventsOnly";
        viewType?: "Day" | "Days" | "Week" | "WorkWeek" | "Resources";
        visible?: boolean;
        watchWidthChanges?: boolean;
        weekStarts?: "Auto" | number;
        width?: string;
        xssProtection?: "Enabled" | "Disabled";

        onAfterCellRender?: EventHandler<CalendarAfterCellRenderArgs>;
        onAfterEventRender?: EventHandler<CalendarAfterEventRenderArgs>;
        onAfterRender?: EventHandler<CalendarAfterRenderArgs>;
        onAjaxError?: EventHandler<CalendarAjaxErrorArgs>;
        onAutoRefresh?: EventHandler<CalendarAutoRefreshArgs>;
        onBeforeCellExport?: EventHandler<CalendarBeforeCellExportArgs>;
        onBeforeCellRender?: EventHandler<CalendarBeforeCellRenderArgs>;
        onBeforeCornerDomAdd?: EventHandler<CalendarBeforeCornerDomAddArgs>;
        onBeforeCornerDomRemove?: EventHandler<CalendarBeforeCornerDomRemoveArgs>;
        onBeforeCornerRender?: EventHandler<CalendarBeforeCornerRenderArgs>;
        onBeforeEventDomAdd?: EventHandler<CalendarBeforeEventDomAddArgs>;
        onBeforeEventDomRemove?: EventHandler<CalendarBeforeEventDomRemoveArgs>;
        onBeforeEventExport?: EventHandler<CalendarBeforeEventExportArgs>;
        onBeforeEventRender?: EventHandler<CalendarBeforeEventRenderArgs>;
        onBeforeHeaderDomAdd?: EventHandler<CalendarBeforeHeaderDomAddArgs>;
        onBeforeHeaderDomRemove?: EventHandler<CalendarBeforeHeaderDomRemoveArgs>;
        onBeforeHeaderExport?: EventHandler<CalendarBeforeHeaderExportArgs>;
        onBeforeHeaderRender?: EventHandler<CalendarBeforeHeaderRenderArgs>;
        onBeforeTimeHeaderDomAdd?: EventHandler<CalendarBeforeTimeHeaderDomAddArgs>;
        onBeforeTimeHeaderDomRemove?: EventHandler<CalendarBeforeTimeHeaderDomRemoveArgs>;
        onBeforeTimeHeaderRender?: EventHandler<CalendarBeforeTimeHeaderRenderArgs>;
        onColumnFilter?: EventHandler<CalendarColumnFilterArgs>;
        onEventClick?: EventHandler<CalendarEventClickArgs>;
        onEventClicked?: EventHandler<CalendarEventClickedArgs>;
        onEventDoubleClick?: EventHandler<CalendarEventDoubleClickArgs>;
        onEventDoubleClicked?: EventHandler<CalendarEventDoubleClickedArgs>;
        onEventEdit?: EventHandler<CalendarEventEditArgs>;
        onEventEdited?: EventHandler<CalendarEventEditedArgs>;
        onEventFilter?: EventHandler<CalendarEventFilterArgs>;
        onEventRightClick?: EventHandler<CalendarEventRightClickArgs>;
        onEventRightClicked?: EventHandler<CalendarEventRightClickedArgs>;
        onEventDelete?: EventHandler<CalendarEventDeleteArgs>;
        onEventDeleted?: EventHandler<CalendarEventDeletedArgs>;
        onEventMove?: EventHandler<CalendarEventMoveArgs>;
        onEventMoved?: EventHandler<CalendarEventMovedArgs>;
        onEventMoving?: EventHandler<CalendarEventMovingArgs>;
        onEventResize?: EventHandler<CalendarEventResizeArgs>;
        onEventResized?: EventHandler<CalendarEventResizedArgs>;
        onEventResizing?: EventHandler<CalendarEventResizingArgs>;
        onEventSelect?: EventHandler<CalendarEventSelectArgs>;
        onEventSelected?: EventHandler<CalendarEventSelectedArgs>;
        onHeaderClick?: EventHandler<CalendarHeaderClickArgs>;
        onHeaderClicked?: EventHandler<CalendarHeaderClickedArgs>;
        onTimeRangeSelect?: EventHandler<CalendarTimeRangeSelectArgs>;
        onTimeRangeSelected?: EventHandler<CalendarTimeRangeSelectedArgs>;
        onTimeRangeDoubleClick?: EventHandler<CalendarTimeRangeDoubleClickArgs>;
        onTimeRangeDoubleClicked?: EventHandler<CalendarTimeRangeDoubleClickedArgs>;


    }

    class CalendarConfig extends CalendarPropsAndEvents {
        events?: EventData[];
        columns?: CalendarColumnData[];
    }

    namespace Calendar {
        function makeDraggable(options: CalendarMakeDraggableOptions): void;
        function registerDropTarget(options: CalendarRegisterDropTargetOptions): void;
    }

    interface CalendarRegisterDropTargetOptions {
        element: HTMLElement;
        onDrop?: EventHandler<CalendarDropTargetDropArgs>;
        onDragOver?: EventHandler<CalendarDropTargetDragOverArgs>;
        onDragLeave?: EventHandler<CalendarDropTargetDragLeaveArgs>;
    }

    interface CalendarDropTargetDropArgs {
        data: CalendarMakeDraggableData;
    }
    interface CalendarDropTargetDragOverArgs {
        data: CalendarMakeDraggableData;
    }
    interface CalendarDropTargetDragLeaveArgs {
        data: CalendarMakeDraggableData;
    }

    interface CalendarMakeDraggableOptions {
        element: HTMLElement;
        keepElement?: boolean;

        data?: CalendarMakeDraggableData;

        id?: EventId;
        text?: string;
        duration?: number | DayPilot.Duration;

        externalCssClass?: string;
        externalHtml?: string;
        externalCursor?: string;
    }

    interface CalendarMakeDraggableData {
        id: EventId;
        text: string;
        duration: number | DayPilot.Duration;
    }

    interface CalendarAfterRenderArgs {
    }

    interface CalendarAfterCellRenderArgs {
        readonly cell: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly resource: ResourceId;
        };
        readonly div: HTMLElement;
    }

    interface CalendarAfterEventRenderArgs {
        readonly e: DayPilot.Event;
        readonly div: HTMLElement;
    }

    interface CalendarAjaxErrorArgs {
        readonly request: XMLHttpRequest;
    }

    interface CalendarAutoRefreshArgs {
        readonly i: number;
        preventDefault(): void;
    }

    interface CalendarBeforeCellRenderArgs {
        readonly cell: {
            start: DayPilot.Date;
            end: DayPilot.Date;
            resource: ResourceId;
            properties: {
                cssClass: string;
                html: string;
                backImage: string;
                backRepeat: string;
                backColor: string;
                business: boolean;
                areas: AreaData[];
                disabled: boolean;
            };
        };
    }

    interface CalendarBeforeCellExportArgs {
        readonly cell: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly resource: ResourceId;
        };
        backColor: string;
    }

    interface CalendarBeforeCornerDomAddArgs {
        element: any;
    }

    interface CalendarBeforeCornerDomRemoveArgs {
        readonly element: any;
    }

    interface CalendarBeforeCornerRenderArgs {
        html: string;
    }

    interface CalendarBeforeHeaderDomAddArgs {
        readonly header: {
            readonly id: ResourceId;
            readonly start: DayPilot.Date;
            readonly name: string;
            readonly children: CalendarColumnData[];
        };
        element: any;
    }

    interface CalendarBeforeHeaderDomRemoveArgs {
        readonly header: {
            readonly id: ResourceId;
            readonly start: DayPilot.Date;
            readonly name: string;
            readonly children: CalendarColumnData[];
        };
        readonly element: any;
    }

    interface CalendarBeforeHeaderExportArgs {
        text: string;
        fontSize: string;
        fontFamily: string;
        fontStyle: FontStyle;
        fontColor: string;
        backColor: string;
        horizontalAlignment: HorizontalAlignment;
        verticalAlignment: VerticalAlignment;

        readonly header: {
            readonly id: ResourceId;
            readonly start: DayPilot.Date;
            readonly name: string;
            readonly level: number;
        };
    }

    interface CalendarBeforeHeaderRenderArgs {
        readonly header: {
            readonly id: ResourceId;
            readonly start: DayPilot.Date;
            readonly name: string;
            readonly children: CalendarColumnData[];
            html: string;
            backColor: string;
            toolTip: string;
            areas: AreaData[];
        };
    }

    interface CalendarBeforeTimeHeaderDomAddArgs {
        readonly header: {
            readonly hours: number;
            readonly minutes: number;
            readonly start: string;
            readonly time: DayPilot.Duration;
        };
        element: any;
    }

    interface CalendarBeforeTimeHeaderDomRemoveArgs {
        readonly header: {
            readonly hours: number;
            readonly minutes: number;
            readonly start: string;
            readonly time: DayPilot.Duration;
        };
        readonly element: any;
    }

    interface CalendarBeforeTimeHeaderRenderArgs {
        readonly header: {
            readonly hours: number;
            readonly minutes: number;
            readonly start: string;
            html: string;
            areas: AreaData[];
            readonly time: DayPilot.Duration;
        };
    }

    interface CalendarBeforeEventDomAddArgs {
        readonly control: DayPilot.Calendar;
        readonly e: DayPilot.Event;
        element: any;
    }

    interface CalendarBeforeEventDomRemoveArgs {
        readonly control: DayPilot.Calendar;
        readonly e: DayPilot.Event;
        readonly element: any;
    }

    interface CalendarBeforeEventExportArgs {
        readonly e: DayPilot.Event;
        text: string;
        fontSize: string;
        fontFamily: string;
        fontStyle: FontStyle;
        fontColor: string;
        backColor: string;
        borderColor: string;
        horizontalAlignment: HorizontalAlignment;
        barWidth: number;
        barColor: string;
        barBackColor: string;
    }

    interface CalendarBeforeEventRenderArgs {
        readonly data: EventData;
    }

    interface CalendarEventClickArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Calendar;
        readonly ctrl: boolean;
        readonly meta: boolean;
        readonly originalEvent: MouseEvent;
        preventDefault(): void;
    }

    interface CalendarEventClickedArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Calendar;
        readonly ctrl: boolean;
        readonly meta: boolean;
        readonly originalEvent: MouseEvent;
    }

    interface CalendarEventDoubleClickArgs {
        readonly e: DayPilot.Event;
        preventDefault(): void;
    }

    interface CalendarEventDoubleClickedArgs {
        readonly e: DayPilot.Event;
    }

    interface CalendarEventEditArgs {
        readonly control: DayPilot.Calendar;
        readonly e: DayPilot.Event;
        newText: string;
        preventDefault(): void;
    }

    interface CalendarEventEditedArgs {
        readonly control: DayPilot.Calendar;
        readonly e: DayPilot.Event;
        readonly newText: string;
    }

    interface CalendarEventFilterArgs {
        readonly e: DayPilot.Event;
        readonly filterParam: any;
        visible: boolean;
    }

    interface CalendarEventRightClickArgs {
        readonly e: DayPilot.Event;
        preventDefault(): void;
    }

    interface CalendarEventRightClickedArgs {
        readonly e: DayPilot.Event;
    }

    interface CalendarEventDeleteArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Calendar;
        preventDefault(): void;
    }

    interface CalendarEventDeletedArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Calendar;
    }

    interface CalendarEventMoveArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Calendar;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
        readonly newResource: string;
        readonly external: boolean;
        readonly areaData: any;
        readonly ctrl: boolean;
        readonly shift: boolean;
        async: boolean;

        preventDefault(): void;
        hideShadow(): void;
        loaded(): void;
    }

    interface CalendarEventMovedArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Calendar;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
        readonly newResource: string;
        readonly external: boolean;
        readonly areaData: any;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly async: boolean;
    }

    interface CalendarEventMovingArgs {
        readonly e: DayPilot.Event;
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
        readonly external: boolean;
        readonly areaData: any;
        html: string;
        cssClass: string;
        allowed: boolean;
        top: CalendarPositionIndicatorProps;
        bottom: CalendarPositionIndicatorProps;
    }

    interface CalendarEventResizeArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Calendar;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
        async: boolean;

        preventDefault(): void;
        hideShadow(): void;
        loaded(): void;
    }

    interface CalendarEventResizedArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Calendar;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
        readonly async: boolean;
    }

    interface CalendarEventResizingArgs {
        readonly e: DayPilot.Event;
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
        html: string;
        cssClass: string;
        allowed: boolean;
        top: CalendarPositionIndicatorProps;
        bottom: CalendarPositionIndicatorProps;
    }

    interface CalendarEventSelectArgs {
        readonly e: DayPilot.Event;
        readonly selected: boolean;
        readonly meta: boolean;
        readonly ctrl: boolean;
        preventDefault(): void;
    }

    interface CalendarEventSelectedArgs {
        readonly e: DayPilot.Event;
        readonly selected: boolean;
        readonly meta: boolean;
        readonly ctrl: boolean;
    }

    interface CalendarHeaderClickArgs {
        readonly header: {
            readonly id: ResourceId;
            readonly name: string;
            readonly start: DayPilot.Date;
        };
        readonly originalEvent: MouseEvent;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly ctrl: boolean;
        preventDefault(): void;
    }

    interface CalendarHeaderClickedArgs {
        readonly header: {
            readonly id: ResourceId;
            readonly name: string;
            readonly start: DayPilot.Date;
        };
        readonly originalEvent: MouseEvent;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly ctrl: boolean;
    }

    interface CalendarTimeRangeSelectArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
        readonly control: DayPilot.Calendar;
        preventDefault(): void;
    }

    interface CalendarTimeRangeSelectedArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
        readonly control: DayPilot.Calendar;
    }

    interface CalendarTimeRangeDoubleClickArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
        preventDefault(): void;
    }

    interface CalendarTimeRangeDoubleClickedArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
    }

    interface CalendarColumnFilterArgs {
        readonly column: {
            readonly id: ResourceId;
            readonly name: string;
            readonly start: DayPilot.Date;
        };
        readonly filterParam: any;
        visible: boolean;
    }

    interface CalendarPositionIndicatorProps {
        width: number;
        space: number;
        html: string;
        enabled: boolean;
    }

    class Month extends MonthPropsAndEvents {
        v: string;
        events: {
            list: EventData[];
            add(e: DayPilot.Event | EventData): void;
            find(id: string): DayPilot.Event;
            findRecurrent(masterId: string, time: DayPilot.Date | string): DayPilot.Event;
            forRange(start: DayPilot.Date | string, end: DayPilot.Date | string): DayPilot.Event[];
            remove(e: DayPilot.Event): void;
            update(e: DayPilot.Event): void;
        };
        multiselect: {
            add(e: DayPilot.Event, dontRedraw?: boolean): void;
            clear(dontRedraw?: boolean): void;
            events(): DayPilot.Event[];
            redraw(): void;
            remove(e: DayPilot.Event): void;
        };

        constructor(id: string | HTMLElement, options?: MonthConfig);

        clearSelection(): void;

        dispose(): void;

        exportAs(format?: "svg" | "png" | "jpeg", options?: any): Export;

        hide(): void;

        init(): void;

        message(msg: string, options?: { delay?: number, cssClass?: string, rawHtml?: boolean }): void;

        show(): void;

        update(options?: MonthConfig): void;

        visibleStart(): DayPilot.Date;

        visibleEnd(): DayPilot.Date;

    }

    class MonthPropsAndEvents {
        allowMultiSelect?: boolean;
        autoRefreshEnabled?: boolean;
        autoRefreshCommand?: string;
        autoRefreshInterval?: number;
        autoRefreshMaxCount?: number;
        backendUrl?: string;
        bubble?: DayPilot.Bubble;
        cellHeaderHeight?: number;
        cellHeight?: number;
        cellMarginBottom?: number;
        cellMode?: boolean;
        clientState?: any;
        contextMenu?: DayPilot.Menu;
        eventClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Select" | "ContextMenu" | "Bubble";
        eventDoubleClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Select" | "Bubble";
        eventEndTime?: boolean;
        eventHeight?: number;
        eventDeleteHandling?: "Update" | "CallBack" | "Disabled";
        eventHoverHandling?: "Bubble" | "Disabled";
        eventMoveHandling?: "Update" | "CallBack" | "Notify" | "Disabled";
        eventMoveToPosition?: boolean;
        eventResizeHandling?: "Update" | "CallBack" | "Notify" | "Disabled";
        eventRightClickHandling?: "ContextMenu" | "Enabled" | "Disabled" | "CallBack" | "Bubble";
        eventSelectHandling?: "Update" | "CallBack" | "Disabled";
        eventStartTime?: boolean;
        headerClickHandling?: "Enabled" | "Disabled" | "CallBack";
        headerHeight?: number;
        hideUntilInit?: boolean;
        lineSpace?: number;
        loadingLabelText?: string;
        loadingLabelHtml?: string;
        loadingLabelVisible?: boolean;
        locale?: string;
        maxEvents?: "All" | number;
        messageHideAfter?: number;
        notifyCommit?: "Immediate" | "Queue";
        showWeekend?: boolean;
        showToolTip?: boolean;
        startDate?: DayPilot.Date | string;
        theme?: string;
        timeFormat?: "Auto" | "Clock12Hours" | "Clock24Hours";
        timeRangeDoubleClickHandling?: "Enabled" | "Disabled" | "CallBack";
        timeRangeSelectedHandling?: "Enabled" | "Disabled" | "CallBack";
        viewType?: "Month" | "Weeks";
        visible?: boolean;
        weekStarts?: "Auto" | number;
        weeks?: number;
        width?: string;
        xssProtection?: "Enabled" | "Disabled";

        onAjaxError?: EventHandler<MonthAjaxErrorArgs>;
        onAfterEventRender?: EventHandler<MonthAfterEventRenderArgs>;
        onAfterRender?: EventHandler<MonthAfterRenderArgs>;
        onBeforeCellExport?: EventHandler<MonthBeforeCellExportArgs>;
        onBeforeCellRender?: EventHandler<MonthBeforeCellRenderArgs>;
        onBeforeEventExport?: EventHandler<MonthBeforeEventExportArgs>;
        onBeforeEventRender?: EventHandler<MonthBeforeEventRenderArgs>;
        onBeforeHeaderExport?: EventHandler<MonthBeforeHeaderExportArgs>;
        onBeforeHeaderRender?: EventHandler<MonthBeforeHeaderRenderArgs>;
        onEventClick?: EventHandler<MonthEventClickArgs>;
        onEventClicked?: EventHandler<MonthEventClickedArgs>;
        onEventDoubleClick?: EventHandler<MonthEventDoubleClickArgs>;
        onEventDoubleClicked?: EventHandler<MonthEventDoubleClickedArgs>;
        onEventSelect?: EventHandler<MonthEventSelectArgs>;
        onEventSelected?: EventHandler<MonthEventSelectedArgs>;
        onEventRightClick?: EventHandler<MonthEventRightClickArgs>;
        onEventRightClicked?: EventHandler<MonthEventRightClickedArgs>;
        onEventMove?: EventHandler<MonthEventMoveArgs>;
        onEventMoved?: EventHandler<MonthEventMovedArgs>;
        onEventResize?: EventHandler<MonthEventResizeArgs>;
        onEventResized?: EventHandler<MonthEventResizedArgs>;
        onTimeRangeSelect?: EventHandler<MonthTimeRangeSelectArgs>;
        onTimeRangeSelected?: EventHandler<MonthTimeRangeSelectedArgs>;
        onHeaderClick?: EventHandler<MonthHeaderClickArgs>;
        onHeaderClicked?: EventHandler<MonthHeaderClickedArgs>;
        onTimeRangeDoubleClick?: EventHandler<MonthTimeRangeDoubleClickArgs>;
        onTimeRangeDoubleClicked?: EventHandler<MonthTimeRangeDoubleClickedArgs>;
    }

    class MonthConfig extends MonthPropsAndEvents {
        events?: EventData;
    }

    interface MonthAjaxErrorArgs {
        readonly request: XMLHttpRequest;
    }

    interface MonthAfterEventRenderArgs {
        readonly e: DayPilot.Event;
        readonly div: HTMLElement;
    }

    interface MonthAfterRenderArgs {
        readonly isCallBack: boolean;
        readonly data: any;
    }

    interface MonthBeforeCellExportArgs {
        readonly cell: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
        };
        backColor: string;
        text: string;
        horizontalAlignment: HorizontalAlignment;
    }

    interface MonthBeforeCellRenderArgs {
        readonly cell: {
            start: DayPilot.Date;
            end: DayPilot.Date;
            properties: {
                areas: AreaData[];
                cssClass: string;
                html: string;
                backImage: string;
                backRepeat: string;
                backColor: string;
                business: boolean;
                headerHtml: string;
                headerBackColor: string;
            };
            events(): DayPilot.Event[];
        };

    }

    interface MonthBeforeEventRenderArgs {
        readonly data: EventData;
    }

    interface MonthBeforeEventExportArgs {
        readonly e: DayPilot.Event;
        text: string;
        backColor: string;
        borderColor: string;
        fontSize: string;
        fontFamily: string;
        fontStyle: FontStyle;
        fontColor: string;
        horizontalAlignment: HorizontalAlignment;
    }

    interface MonthBeforeHeaderExportArgs {
        readonly header: {
            readonly dayOfWeek: number;
        };
        text: string;
        backColor: string;
        fontSize: string;
        fontFamily: string;
        fontStyle: FontStyle;
        fontColor: string;
        horizontalAlignment: HorizontalAlignment;
        verticalAlignment: VerticalAlignment;
    }

    interface MonthBeforeHeaderRenderArgs {
        readonly header: {
            readonly dayOfWeek: number;
            html: string;
            backColor: string;
            cssClass: string;
        };
    }

    interface MonthEventClickArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Month;
        readonly div: HTMLElement;
        readonly originalEvent: MouseEvent;
        readonly meta: boolean;
        readonly ctrl: boolean;
        preventDefault(): void;
    }

    interface MonthEventClickedArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Month;
        readonly div: HTMLElement;
        readonly originalEvent: MouseEvent;
        readonly meta: boolean;
        readonly ctrl: boolean;
    }

    interface MonthEventDoubleClickArgs {
        readonly e: DayPilot.Event;
        preventDefault(): void;
    }

    interface MonthEventDoubleClickedArgs {
        readonly e: DayPilot.Event;
    }

    interface MonthEventSelectArgs {
        readonly e: DayPilot.Event;
        readonly selected: boolean;
        readonly meta: boolean;
        readonly ctrl: boolean;
        preventDefault(): void;
    }

    interface MonthEventSelectedArgs {
        readonly e: DayPilot.Event;
        readonly selected: boolean;
        readonly meta: boolean;
        readonly ctrl: boolean;
    }

    interface MonthEventRightClickArgs {
        readonly e: DayPilot.Event;
        preventDefault(): void;
    }

    interface MonthEventRightClickedArgs {
        readonly e: DayPilot.Event;
    }

    interface MonthEventMoveArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Month;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
        readonly position: number;
        readonly ctrl: boolean;
        readonly external: boolean;
        readonly shift: boolean;
        preventDefault(): void;
    }

    interface MonthEventMovedArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Month;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
        readonly position: number;
        readonly ctrl: boolean;
        readonly external: boolean;
        readonly shift: boolean;
    }

    interface MonthEventResizeArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Month;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
        preventDefault(): void;
    }

    interface MonthEventResizedArgs {
        readonly e: DayPilot.Event;
        readonly control: DayPilot.Month;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
    }

    interface MonthTimeRangeSelectArgs {
        readonly control: DayPilot.Month;
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        preventDefault(): void;
    }

    interface MonthTimeRangeSelectedArgs {
        readonly control: DayPilot.Month;
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
    }

    interface MonthHeaderClickArgs {
        readonly header: {
            readonly dayOfWeek: number;
        };
        preventDefault(): void;
    }

    interface MonthHeaderClickedArgs {
        readonly header: {
            readonly dayOfWeek: number;
        };
    }

    interface MonthTimeRangeDoubleClickArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        preventDefault(): void;
    }

    interface MonthTimeRangeDoubleClickedArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
    }

    class Kanban extends KanbanPropsAndEvents {
        v: string;
        cards: {
            list: CardData[];
            add(c: DayPilot.Card): void;
            remove(c: DayPilot.Card): void;
            update(c: DayPilot.Card): void;
        };
        columns: {
            list: KanbanColumnData[];
        };
        swimlanes: {
            list: SwimlaneData[];
        };

        constructor(id: string, options?: KanbanConfig);

        hide(): void;

        init(): void;

        dispose(): void;

        message(msg: string, options?: { delay?: number, cssClass?: string }): void;

        show(): void;

        update(options?: KanbanConfig): void;

    }

    class KanbanPropsAndEvents {
        barWidth?: number;
        cardDeleteHandling?: "Disabled" | "Update";
        cardMarginBottom?: number;
        cardMarginLeft?: number;
        cardMarginRight?: number;
        cardMoveHandling?: "Update" | "Disabled";
        cellMarginBottom?: number;
        cellMarginTop?: number;
        columnHeaderHeight?: number;
        columnMoveHandling?: "Disabled" | "Update";
        columnWidthSpec?: "Auto" | "Fixed";
        crosshairColor?: string;
        height?: number;
        heightSpec?: "Auto" | "Max" | "Fixed" | "Parent100Pct";
        rowMinHeight?: number;
        swimlaneCollapsingEnabled?: boolean;
        swimlaneHeaderWidth?: number;
        swimlaneMoveHandling?: "Disabled" | "Update";
        theme?: string;
        visible?: boolean;

        onCardClick?: EventHandler<KanbanCardClickArgs>;
        onCardClicked?: EventHandler<KanbanCardClickedArgs>;
        onCardDelete?: EventHandler<KanbanCardDeleteArgs>;
        onCardDeleted?: EventHandler<KanbanCardDeletedArgs>;
        onCardMove?: EventHandler<KanbanCardMoveArgs>;
        onCardMoved?: EventHandler<KanbanCardMovedArgs>;
        onColumnMove?: EventHandler<KanbanColumnMoveArgs>;
        onColumnMoved?: EventHandler<KanbanColumnMovedArgs>;
        onHeightChanged?: EventHandler<KanbanHeightChangedArgs>;
        onSwimlaneMove?: EventHandler<KanbanSwimlaneMoveArgs>;
        onSwimlaneMoved?: EventHandler<KanbanSwimlaneMovedArgs>;
    }

    class KanbanConfig extends KanbanPropsAndEvents {
        cards?: CardData[];
        columns?: KanbanColumnData[];
        swimlanes?: SwimlaneData[];
    }

    interface KanbanCardClickArgs {
        readonly card: DayPilot.Card;
        preventDefault(): void;
    }

    interface KanbanCardClickedArgs {
        readonly card: DayPilot.Card;
    }

    interface KanbanCardDeleteArgs {
        readonly card: DayPilot.Card;
        readonly control: DayPilot.Kanban;
        preventDefault(): void;
    }

    interface KanbanCardDeletedArgs {
        readonly card: DayPilot.Card;
        readonly control: DayPilot.Kanban;
    }

    interface KanbanCardMoveArgs {
        readonly control: DayPilot.Kanban;
        readonly card: DayPilot.Card;
        readonly column: { readonly data: KanbanColumnData; };
        readonly position: number;
        readonly external: boolean;
        readonly swimlane: { readonly data: SwimlaneData; };
        readonly previous: DayPilot.Card;
        readonly next: DayPilot.Card;
        preventDefault(): void;
    }

    interface KanbanCardMovedArgs {
        readonly control: DayPilot.Kanban;
        readonly card: DayPilot.Card;
        readonly column: { readonly data: KanbanColumnData; };
        readonly position: number;
        readonly external: boolean;
        readonly swimlane: { readonly data: SwimlaneData; };
        readonly previous: DayPilot.Card;
        readonly next: DayPilot.Card;
    }

    interface KanbanColumnMoveArgs {
        readonly column: { readonly data: KanbanColumnData; };
        readonly position: number;
        readonly previous: { readonly data: KanbanColumnData; };
        readonly next: { readonly data: KanbanColumnData; };
        preventDefault(): void;
    }

    interface KanbanColumnMovedArgs {
        readonly column: { readonly data: KanbanColumnData; };
        readonly position: number;
        readonly previous: { readonly data: KanbanColumnData; };
        readonly next: { readonly data: KanbanColumnData; };
    }

    interface KanbanHeightChangedArgs {
        readonly oldHeight: number;
        readonly newHeight: number;
    }

    interface KanbanSwimlaneMoveArgs {
        readonly control: DayPilot.Kanban;
        readonly swimlane: { readonly data: SwimlaneData; };
        readonly position: number;
        readonly previous: { readonly data: SwimlaneData; };
        readonly next: { readonly data: SwimlaneData; };
        preventDefault(): void;
    }

    interface KanbanSwimlaneMovedArgs {
        readonly control: DayPilot.Kanban;
        readonly swimlane: { readonly data: SwimlaneData; };
        readonly position: number;
        readonly previous: { readonly data: SwimlaneData; };
        readonly next: { readonly data: SwimlaneData; };
    }

    class Gantt extends GanttPropsAndEvents {
        v: string;
        links: {
            list: LinkData[];
            add(link: DayPilot.Link): void;
            find(id: string): DayPilot.Link;
            findByFromTo(from: DayPilot.Date | string, to: DayPilot.Date | string): DayPilot.Link;
            remove(link: DayPilot.Link): void;
        };
        rows: {
            selection: {
                add(task: DayPilot.Task): void;
                clear(): void;
                get(): DayPilot.Task[];
            };
        };
        tasks: {
            list: TaskData[];
            add(task: DayPilot.Task): void;
            find(id: string): DayPilot.Task;
            remove(task: DayPilot.Task): void;
            update(task: DayPilot.Task): void;
        };

        constructor(id: string | HTMLElement, options?: GanttConfig);

        commandCallBack(command: string, data?: any): void;

        init(): void;

        dispose(): void;

        message(html: string): void;

        scrollTo(date: DayPilot.Date, animated?: "fast" | "normal" | "slow" | "linear" | number, position?: "left" | "middle" | "right"): void;

        scrollTo(date: string, animated?: "fast" | "normal" | "slow" | "linear" | number, position?: "left" | "middle" | "right"): void;

        scrollTo(pixels: number, animated?: "fast" | "normal" | "slow" | "linear" | number, position?: "left" | "middle" | "right"): void;

        setHeight(pixels: number): void;

        update(options?: GanttConfig): void;

    }

    class GanttPropsAndEvents {
        autoRefreshCommand?: string;
        autoRefreshEnabled?: boolean;
        autoRefreshInterval?: number;
        autoRefreshMaxCount?: number;
        autoScroll?: "Drag" | "Always" | "Disabled";
        bubbleCell?: DayPilot.Bubble;
        bubbleRow?: DayPilot.Bubble;
        bubbleTask?: DayPilot.Bubble;
        cellDuration?: number;
        cellGroupBy?: GroupBy;
        cellWidth?: number;
        cellWidthSpec?: "Auto" | "Fixed";
        columns?: GanttColumnData[];
        completeBarHeight?: number;
        completeBarVisible?: boolean;
        contextMenuTask?: DayPilot.Menu;
        contextMenuLink?: DayPilot.Menu;
        contextMenuRow?: DayPilot.Menu;
        cornerText?: string;
        cornerHtml?: string;
        crosshairType?: "Full" | "Header" | "Disabled";
        days?: number;
        doubleClickTimeout?: number;
        floatingTasks?: boolean;
        floatingTimeHeaders?: boolean;
        headerHeight?: number;
        height?: number;
        heightSpec?: "Auto" | "Max" | "Fixed" | "Parent100Pct" | "Max100Pct";
        hideBorderFor100PctHeight?: boolean;
        hideUntilInit?: boolean;
        linkBottomMargin?: number;
        linkCreateHandling?: "Disabled" | "Update" | "CallBack" | "Notify";
        linkPointSize?: number;
        loadingLabelText?: string;
        loadingLabelHtml?: string;
        loadingLabelVisible?: boolean;
        locale?: string | DayPilot.Locale;
        messageBarPosition?: "Top" | "Bottom";
        messageHideAfter?: number;
        progressiveRowRendering?: boolean;
        progressiveRowRenderingPreload?: number;
        progressiveTaskRendering?: "Progressive" | "Disabled";
        progressiveTaskRenderingMargin?: number;
        progressiveTaskRenderingCacheSize?: number;
        progressiveTaskRenderingCacheSweeping?: boolean;
        rowClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select";
        rowCreateHandling?: "Disabled" | "Enabled" | "CallBack";
        rowDoubleClickHandling?: "Disabled" | "Enabled" | "CallBack" | "Select" | "Edit";
        rowEditHandling?: "Update" | "CallBack";
        rowHeaderHideIconEnabled?: boolean;
        rowHeaderScrolling?: boolean;
        rowHeaderSplitterWidth?: number;
        rowHeaderWidth?: number;
        rowHeaderWidthAutoFit?: boolean;
        rowHeaderWidthAutoFitShrink?: boolean;
        rowMarginBottom?: number;
        rowMinHeight?: number;
        rowMoveHandling?: "Disabled" | "Update" | "CallBack" | "Notify";
        rowSelectHandling?: "Update" | "CallBack" | "Notify";
        scale?: "Manual" | "CellDuration" | "Minute" | "Hour" | "Day" | "Week" | "Month" | "Year";
        scrollDelayCells?: number;
        scrollDelayTasks?: number;
        scrollDelayFloats?: number;
        selectedRows?: string[] | number[];
        separators?: SeparatorData[];
        snapToGrid?: boolean;
        startDate?: DayPilot.Date | string;
        tapAndHoldTimeout?: number;
        taskClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select" | "ContextMenu" | "Bubble";
        taskDoubleClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select" | "ContextMenu" | "Bubble";
        taskGroupMode?: "Auto" | "Manual";
        taskHeight?: number;
        taskHtmlLeftMargin?: number;
        taskHtmlRightMargin?: number;
        taskMoveHandling?: "Update" | "Disabled" | "CallBack" | "Notify";
        taskMovingStartEndEnabled?: boolean;
        taskMovingStartEndFormat?: boolean;
        taskResizeHandling?: "Update" | "Disabled" | "CallBack" | "Notify";
        taskResizeMargin?: number;
        taskResizingStartEndEnabled?: boolean;
        taskResizingStartEndFormat?: boolean;
        taskRightClickHandling?: "Enabled" | "Disabled" | "CallBack" | "Edit" | "Select" | "ContextMenu" | "Bubble";
        tasksLoadMethod?: "GET" | "POST";
        taskVersionHeight?: number;
        taskVersionMargin?: number;
        taskVersionPosition?: "Above" | "Below";
        taskVersionsEnabled?: boolean;
        theme?: string;
        timeHeaders?: TimeHeaderData[];
        timeline?: TimelineData[];
        treeAutoExpand?: boolean;
        treeIndent?: number;
        treeImageMarginLeft?: number;
        treeImageMarginTop?: number;
        treeImageMarginRight?: number;
        treeImageWidth?: number;
        treeImageHeight?: number;
        useEventBoxes?: "Always" | "Never" | "ShortEventsOnly";
        visible?: boolean;
        xssProtection?: "Enabled" | "Disabled";


        onAfterRender?: EventHandler<GanttAfterRenderArgs>;
        onBeforeCellRender?: EventHandler<GanttBeforeCellRenderArgs>;
        onBeforeCornerRender?: EventHandler<GanttBeforeCornerRenderArgs>;
        onBeforeRowHeaderRender?: EventHandler<GanttBeforeRowHeaderRenderArgs>;
        onBeforeTaskRender?: EventHandler<GanttBeforeTaskRenderArgs>;
        onBeforeTimeHeaderRender?: EventHandler<GanttBeforeTimeHeaderRenderArgs>;
        onColumnResized?: EventHandler<GanttColumnResizedArgs>;
        onLinkCreate?: EventHandler<GanttLinkCreateArgs>;
        onLinkCreated?: EventHandler<GanttLinkCreatedArgs>;
        onRowClick?: EventHandler<GanttRowClickArgs>;
        onRowClicked?: EventHandler<GanttRowClickedArgs>;
        onRowCreate?: EventHandler<GanttRowCreateArgs>;
        onRowCreated?: EventHandler<GanttRowCreatedArgs>;
        onRowDoubleClick?: EventHandler<GanttRowDoubleClickArgs>;
        onRowDoubleClicked?: EventHandler<GanttRowDoubleClickedArgs>;
        onRowEdit?: EventHandler<GanttRowEditArgs>;
        onRowEdited?: EventHandler<GanttRowEditedArgs>;
        onRowMove?: EventHandler<GanttRowMoveArgs>;
        onRowMoved?: EventHandler<GanttRowMovedArgs>;
        onRowMoving?: EventHandler<GanttRowMovingArgs>;
        onRowSelect?: EventHandler<GanttRowSelectArgs>;
        onRowSelected?: EventHandler<GanttRowSelectedArgs>;
        onTaskClick?: EventHandler<GanttTaskClickArgs>;
        onTaskClicked?: EventHandler<GanttTaskClickedArgs>;
        onTaskDoubleClick?: EventHandler<GanttTaskDoubleClickArgs>;
        onTaskDoubleClicked?: EventHandler<GanttTaskDoubleClickedArgs>;
        onTaskMove?: EventHandler<GanttTaskMoveArgs>;
        onTaskMoved?: EventHandler<GanttTaskMovedArgs>;
        onTaskMoving?: EventHandler<GanttTaskMovingArgs>;
        onTaskResize?: EventHandler<GanttTaskResizeArgs>;
        onTaskResized?: EventHandler<GanttTaskResizedArgs>;
        onTaskResizing?: EventHandler<GanttTaskResizingArgs>;
        onTaskRightClick?: EventHandler<GanttTaskRightClickArgs>;
        onTaskRightClicked?: EventHandler<GanttTaskRightClickedArgs>;
    }

    class GanttConfig extends GanttPropsAndEvents {
        tasks?: TaskData[];
        links?: LinkData[];
    }

    interface GanttColumnData {
        title?: string;
        width?: number;
        property?: string;
    }

    interface GanttAfterRenderArgs {
        readonly isCallBack: boolean;
        readonly isScroll: boolean;
        readonly data: any;
    }

    interface GanttBeforeCellRenderArgs {
        readonly control: Gantt;
        readonly cell: Cell;
        readonly task: DayPilot.Task;
    }

    interface GanttBeforeCornerRenderArgs {
        readonly control: Gantt;
        html: string;
        areas: AreaData[];
    }

    interface GanttBeforeRowHeaderRenderArgs {
        readonly task: DayPilot.Task;
        readonly row: DayPilot.Row;
    }

    interface GanttBeforeTaskRenderArgs {
        readonly data: TaskData;
        readonly type: TaskType;
    }

    interface GanttBeforeTimeHeaderRenderArgs {
        readonly control: Gantt;
        readonly header: {
            readonly start: DayPilot.Date;
            readonly end: DayPilot.Date;
            readonly level: number;
            text: string;
            html: string;
            toolTip: string;
            backColor: string;
            fontColor: string;
            cssClass: string;
        };
    }

    interface GanttColumnResizedArgs {
        readonly column: GanttColumnData;
    }

    interface GanttLinkCreateArgs {
        readonly source: Task;
        readonly target: Task;
        readonly control: Gantt;
        readonly type: LinkType;
        preventDefault(): void;
    }

    interface GanttLinkCreatedArgs {
        readonly source: Task;
        readonly target: Task;
        readonly control: Gantt;
        readonly type: LinkType;
    }

    interface GanttRowClickArgs {
        readonly task: Task;
        preventDefault(): void;
    }

    interface GanttRowClickedArgs {
        readonly task: Task;
    }

    interface GanttRowCreateArgs {
        readonly control: Gantt;
        text: string;
        preventDefault(): void;
    }

    interface GanttRowCreatedArgs {
        readonly control: Gantt;
        readonly text: string;
    }

    interface GanttRowDoubleClickArgs {
        readonly task: Task;
        preventDefault(): void;
    }

    interface GanttRowDoubleClickedArgs {
        readonly task: Task;
    }

    interface GanttRowEditArgs {
        readonly task: Task;
        readonly canceled: boolean;
        newText: string;
        async: boolean;

        loaded(): void;
        preventDefault(): void;
    }

    interface GanttRowEditedArgs {
        readonly task: Task;
        readonly newText: string;
        readonly async: boolean;
    }

    interface GanttRowMoveArgs {
        readonly source: Task;
        readonly target: Task;
        readonly control: Gantt;
        position: "child" | "before" | "after" | "forbidden";
        preventDefault(): void;
    }

    interface GanttRowMovedArgs {
        readonly source: Task;
        readonly target: Task;
        readonly control: Gantt;
        readonly position: "child" | "before" | "after" | "forbidden";
    }

    interface GanttRowMovingArgs {
        readonly source: Task;
        readonly target: Task;
        position: "child" | "before" | "after" | "forbidden";
    }

    interface GanttRowSelectArgs {
        readonly task: Task;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly selected: boolean;
        preventDefault(): void;
    }

    interface GanttRowSelectedArgs {
        readonly task: Task;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly selected: boolean;
    }

    interface GanttTaskClickArgs {
        readonly task: DayPilot.Task;
        readonly ctrl: boolean;
        readonly meta: boolean;
        readonly shift: boolean;
        readonly control: DayPilot.Gantt;
        readonly originalEvent: MouseEvent;
        preventDefault(): void;
    }

    interface GanttTaskClickedArgs {
        readonly task: DayPilot.Task;
        readonly ctrl: boolean;
        readonly meta: boolean;
        readonly shift: boolean;
        readonly control: DayPilot.Gantt;
        readonly originalEvent: MouseEvent;
    }

    interface GanttTaskDoubleClickArgs {
        readonly task: DayPilot.Task;
        readonly control: DayPilot.Gantt;
        readonly originalEvent: MouseEvent;
        preventDefault(): void;
    }

    interface GanttTaskDoubleClickedArgs {
        readonly task: DayPilot.Task;
        readonly control: DayPilot.Gantt;
        readonly originalEvent: MouseEvent;
    }

    interface GanttTaskMoveArgs {
        readonly control: DayPilot.Gantt;
        readonly task: DayPilot.Task;
        readonly external: boolean;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        async: boolean;
        newStart: DayPilot.Date;
        newEnd: DayPilot.Date;

        loaded(): void;
        preventDefault(): void;
    }

    interface GanttTaskMovedArgs {
        readonly control: DayPilot.Gantt;
        readonly task: DayPilot.Task;
        readonly external: boolean;
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly async: boolean;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
    }

    interface GanttTaskMovingArgs {
        readonly ctrl: boolean;
        readonly shift: boolean;
        readonly meta: boolean;
        readonly alt: boolean;
        readonly duration: DayPilot.Duration;
        readonly task: DayPilot.Task;
        readonly external: boolean;
        allowed: boolean;
        cssClass: string;
        start: DayPilot.Date;
        end: DayPilot.Date;
        html: string;
        left: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
        right: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
    }

    interface GanttTaskResizeArgs {
        readonly control: DayPilot.Gantt;
        readonly task: DayPilot.Task;
        readonly what: "start" | "end";
        async: boolean;
        newStart: DayPilot.Date;
        newEnd: DayPilot.Date;

        loaded(): void;
        preventDefault(): void;
    }

    interface GanttTaskResizedArgs {
        readonly control: DayPilot.Gantt;
        readonly task: DayPilot.Task;
        readonly what: "start" | "end";
        readonly async: boolean;
        readonly newStart: DayPilot.Date;
        readonly newEnd: DayPilot.Date;
    }

    interface GanttTaskResizingArgs {
        readonly duration: DayPilot.Duration;
        readonly task: DayPilot.Task;
        readonly shift: boolean;
        readonly ctrl: boolean;
        readonly alt: boolean;
        readonly meta: boolean;
        readonly what: "start" | "end";
        readonly anchor: DayPilot.Date;
        readonly control: DayPilot.Gantt;
        readonly left: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
        readonly right: {
            html: string;
            enabled: boolean;
            space: number;
            width: number;
            height: number;
        };
        start: DayPilot.Date;
        end: DayPilot.Date;
        allowed: boolean;
        cssClass: string;
        html: string;
    }

    interface GanttTaskRightClickArgs {
        readonly task: DayPilot.Task;
        readonly originalEvent: MouseEvent;
        preventDefault(): void;
    }

    interface GanttTaskRightClickedArgs {
        readonly task: DayPilot.Task;
        readonly originalEvent: MouseEvent;
    }


    class Queue extends QueuePropsAndEvents {
        constructor(id: string | HTMLElement, options?: QueueConfig);

        v: string;

        init(): void;
        dispose(): void;
        update(options?: QueueConfig): void;

        events: {
            list: QueueData[];
            add(item: QueueData): void;
            add(e: DayPilot.Event): void;
            find(id: string): DayPilot.Event;
            find(id: number): DayPilot.Event;
            remove(id: string): void;
            remove(id: number): void;
            remove(data: QueueData): void;
            remove(e: DayPilot.Event): void;
            update(item: QueueData): void;
            update(e: DayPilot.Event): void;
        };

        multiselect: {
            add(e: DayPilot.Event): void;
            clear(): void;
            get(): DayPilot.Event[];
        };

    }

    class QueuePropsAndEvents {
        contextMenu?: DayPilot.Menu;
        emptyText?: string;
        emptyHtml?: string;
        eventClickHandling?: "Enabled" | "Disabled" | "Select";
        eventRightClickHandling?: "ContextMenu" | "Enabled" | "Disabled";
        eventSelectHandling?: "Update" | "Disabled";
        eventHeight?: number;
        eventTextWrappingEnabled?: boolean;
        lineSpace?: number;
        theme?: string;

        onBeforeEventRender?: EventHandler<any>;
        onEventClick?: EventHandler<any>;
        onEventClicked?: EventHandler<any>;
        onEventRightClick?: EventHandler<any>;
        onEventRightClicked?: EventHandler<any>;
        onEventMove?: EventHandler<any>;
        onEventMoved?: EventHandler<any>;
        onEventSelect?: EventHandler<any>;
        onEventSelected?: EventHandler<any>;
    }

    class QueueConfig extends QueuePropsAndEvents {
        events?: QueueData[];
    }

    class QueueData {
        start?: string | DayPilot.Date;
        end?: string | DayPilot.Date;
        duration?: number | DayPilot.Duration;
        id: EventId;
        text: string;
    }

    class Navigator extends NavigatorPropsAndEvents {
        v: string;
        events: {
            list: EventDataShort[];
        };

        constructor(id: string | HTMLElement, options?: NavigatorConfig);

        init(): void;

        dispose(): void;

        update(options?: NavigatorConfig): void;

        select(date: DayPilot.Date | string): void;

        hide(): void;

        show(): void;

        visibleEnd(): DayPilot.Date;

        visibleStart(): DayPilot.Date;
    }

    class NavigatorPropsAndEvents {
        bound?: string;
        cellHeight?: number;
        cellWidth?: number;
        command?: string;
        dayHeaderHeight?: number;
        freeHandSelectionEnabled?: boolean;
        locale?: string;
        orientation?: "Vertical" | "Horizontal";
        rowsPerMonth?: "Auto" | "Six";
        selectionDay?: DayPilot.Date;
        selectionEnd?: DayPilot.Date;
        selectionStart?: DayPilot.Date;
        selectMode?: "Day" | "Week" | "Month" | "None";
        showMonths?: number;
        showWeekNumbers?: boolean;
        skipMonths?: number;
        startDate?: DayPilot.Date | string;
        theme?: string;
        titleHeight?: number;
        weekStarts?: "Auto" | number;
        weekNumberAlgorithm?: "Auto" | "US" | "ISO8601";
        timeRangeSelectedHandling?: "Bind" | "None";
        visibleRangeChangedHandling?: "Enabled" | "Disabled" | "CallBack";

        onAjaxError?: EventHandler<NavigatorAjaxErrorArgs>;
        onBeforeCellRender?: EventHandler<NavigatorBeforeCellRenderArgs>;
        onTimeRangeSelect?: EventHandler<NavigatorTimeRangeSelectArgs>;
        onTimeRangeSelected?: EventHandler<NavigatorTimeRangeSelectedArgs>;
        onVisibleRangeChange?: EventHandler<NavigatorVisibleRangeChangeArgs>;
        onVisibleRangeChanged?: EventHandler<NavigatorVisibleRangeChangedArgs>;
    }

    class NavigatorConfig extends NavigatorPropsAndEvents {
        events?: EventData[];
    }

    interface NavigatorAjaxErrorArgs {
        readonly request: XMLHttpRequest;
    }

    interface NavigatorBeforeCellRenderArgs {
        readonly cell: {
            readonly day: DayPilot.Date;
            readonly isCurrentMonth: boolean;
            readonly isToday: boolean;
            readonly isWeekend: boolean;
            html: string;
            cssClass: string;
        };
    }

    interface NavigatorTimeRangeSelectArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly days: number;
        readonly mode: "Day" | "Week" | "Month" | "None";
        preventDefault(): void;
    }

    interface NavigatorTimeRangeSelectedArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly days: number;
        readonly mode: "Day" | "Week" | "Month" | "None";
    }

    interface NavigatorVisibleRangeChangeArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        preventDefault(): void;
    }

    interface NavigatorVisibleRangeChangedArgs {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
    }


    class Bubble extends BubblePropsAndEvents {
        v: string;

        constructor(options?: BubbleConfig);

        hide(): void;

        showEvent(e: DayPilot.Event): void;

        showHtml(html: string, element?: HTMLElement): void;
    }

    class BubblePropsAndEvents {
        animated?: boolean;
        animation?: "fast" | "slow" | "jump";
        hideAfter?: number;
        hideOnClick?: boolean;
        hideOnHover?: boolean;
        loadingText?: string;
        position?: "Above" | "Mouse" | "EventTop";
        showAfter?: number;
        showLoadingLabel?: boolean;
        theme?: string;
        zIndex?: number;

        onLoad?: EventHandler<BubbleLoadArgs>;
        onDomAdd?: EventHandler<BubbleDomAddArgs>;
        onDomRemove?: EventHandler<BubbleDomRemoveArgs>;
    }

    class BubbleConfig extends BubblePropsAndEvents {
    }

    namespace Bubble {
        function hide(): void;

        function getActive(): DayPilot.Bubble;
    }

    interface BubbleLoadArgs {
        readonly source: any;
        async: boolean;
        loaded(): void;
        html: string;
    }

    interface BubbleDomAddArgs {
        readonly source: any;
        element: any;
    }

    interface BubbleDomRemoveArgs {
        readonly source: any;
        readonly element: any;
    }

    class Locale {
        datePattern: string;
        dateTimePattern: string;
        dayNames: string[];
        dayNamesShort: string[];
        monthNames: string[];
        monthNamesShort: string[];
        timeFormat: "Clock12Hours" | "Clock24Hours";
        timePattern: string;
        weekStarts: number;

        constructor(id: string, properties: {
            dayNames: string[];
            dayNamesShort: string[];
            monthNames: string[];
            monthNamesShort: string[];
            timePattern: string;
            datePattern: string;
            dateTimePattern: string;
            timeFormat: "Clock12Hours" | "Clock24Hours";
            weekStarts: number;
        });
    }

    namespace Locale {
        function register(locale: DayPilot.Locale): void;

        function find(id: string): DayPilot.Locale;
    }

    class Menu extends MenuPropsAndEvents {
        v: string;
        constructor(options?: MenuConfig);

        show(target?: any): void;
        hide(): void;
    }

    class MenuPropsAndEvents {
        hideOnMouseOut?: boolean;
        items?: MenuItemData[];
        menuTitle?: string;
        onShow?: EventHandler<MenuShowArgs>;
        onHide?: EventHandler<MenuHideArgs>;
        showMenuTitle?: boolean;
        zIndex?: number;
        theme?: string;
    }

    class MenuConfig extends MenuPropsAndEvents {
    }

    interface MenuShowArgs {
        readonly source: any;
        readonly menu: DayPilot.Menu;
        preventDefault(): void;
    }

    interface MenuHideArgs {
    }

    namespace Menu {
        function hide(): void;
    }

    class MenuBar {
        items: any[];

        constructor(id: string, options?: any);

        init(): void;

        dispose(): void;
    }

    class Date {
        constructor(str?: string | DayPilot.Date);
        constructor(date: GlobalDate, isLocal?: boolean);

        addDays(days: number): DayPilot.Date;

        addHours(hours: number): DayPilot.Date;

        addMilliseconds(millis: number): DayPilot.Date;

        addMinutes(minutes: number): DayPilot.Date;

        addMonths(months: number): DayPilot.Date;

        addSeconds(seconds: number): DayPilot.Date;

        addTime(ticks: number): DayPilot.Date;
        addTime(duration: DayPilot.Duration): DayPilot.Date;

        addYears(years: number): DayPilot.Date;

        dayOfWeek(): number;

        dayOfWeekISO(): number;

        dayOfYear(): number;

        daysInMonth(): number;

        daysInYear(): number;

        equals(another: DayPilot.Date): boolean;

        firstDayOfMonth(): DayPilot.Date;

        firstDayOfWeek(locale?: string | DayPilot.Locale): DayPilot.Date;
        firstDayOfWeek(firstDayOfWeek?: number): DayPilot.Date;

        firstDayOfYear(): DayPilot.Date;

        getDatePart(): DayPilot.Date;

        getDay(): number;

        getDayOfWeek(): number;

        getYear(): number;

        getHours(): number;

        getMilliseconds(): number;

        getMinutes(): number;

        getMonth(): number;

        getSeconds(): number;

        getTime(): number;

        getTimePart(): number;

        getTotalTicks(): number;

        getYear(): number;

        lastDayOfMonth(): DayPilot.Date;

        toDate(): GlobalDate;

        toDateLocal(): GlobalDate;

        toString(pattern?: string, locale?: string | DayPilot.Locale): string;

        toStringSortable(): string;

        weekNumber(): number;

        weekNumberISO(): number;
    }

    namespace Date {
        function fromYearMonthDay(year: number, month: number, day: number): DayPilot.Date;

        function parse(input: string, pattern: string, locale?: string | DayPilot.Locale): DayPilot.Date;

        function today(): DayPilot.Date;

        function now(): DayPilot.Date;

        namespace Cache {
            function clear(): void;
        }
    }

    namespace Util {
        function overlaps(start1: DayPilot.Date, end1: DayPilot.Date, start2: DayPilot.Date, end2: DayPilot.Date): boolean;
        function overlaps(start1: number, end1: number, start2: number, end2: number): boolean;
    }

    class Duration {

        ticks: number;

        constructor(ticks: number);

        constructor(start: DayPilot.Date | string, end: DayPilot.Date | string);

        toString(pattern?: string): string;

        totalMilliseconds(): number;

        totalSeconds(): number;

        totalMinutes(): number;

        totalHours(): number;

        totalDays(): number;

        milliseconds(): number;

        seconds(): number;

        minutes(): number;

        hours(): number;

        days(): number;

        add(d: DayPilot.Duration): DayPilot.Duration;
    }

    namespace Duration {
        function ofWeeks(i: number): DayPilot.Duration;

        function ofDays(i: number): DayPilot.Duration;

        function ofHours(i: number): DayPilot.Duration;

        function ofMinutes(i: number): DayPilot.Duration;

        function ofSeconds(i: number): DayPilot.Duration;
    }

    class Event {
        data: any;

        constructor(data: EventData);

        start(): DayPilot.Date;
        start(newStart: DayPilot.Date): void;

        end(): DayPilot.Date;
        end(newEnd: DayPilot.Date): void;

        id(): EventId;

        text(): string;
        text(newText: string): void;

        resource(): string;
        resource(newResource: string): void;

        duration(): DayPilot.Duration;
    }

    class Task {
        data: TaskData;
        row: {
            expand(): void;
            expanded(): boolean;
            collapse(): void;
            toggle(): void;
        }

        constructor(data: TaskData);

        id(): TaskId;

        id(newId: string): void;

        text(): string;

        text(newText: string): void;

        start(): DayPilot.Date;

        start(newStart: DayPilot.Date | string): void;

        end(): DayPilot.Date;

        end(newEnd: DayPilot.Date | string): void;

        complete(): number;

        complete(newComplete: number): void;

        type(): TaskType;

        type(newType: TaskType): void;

        children(): DayPilot.Task[];
    }

    class Card {
        data: CardData;

        constructor(data: CardData);
    }

    class Link {
        data: LinkData;

        constructor(data: LinkData);
    }

    class Row {
        events: {
            all(): DayPilot.Event[];
            isEmpty(): boolean;
            forRange(start: string | DayPilot.Date, end: string | DayPilot.Date): DayPilot.Event[];
            totalDuration(): DayPilot.Duration;
        };
        cells: {
            all(): CellArray;
            forRange(start: string | DayPilot.Date, end: string | DayPilot.Date): CellArray;
            totalDuration(): DayPilot.Duration;
        };
        groups: {
            all(): EventGroup[];
            collapseAll(): void;
            collapsed(): EventGroup[];
            expandAll(): void;
            expanded(): EventGroup[];
        };
        id: string;
        name: string;
        start: DayPilot.Date;
        data: any;
        index: number;
        displayY: number;
        hidden: boolean;
        hiddenUsingFilter: boolean;

        addClass(className: string): void;

        children(): DayPilot.Row[];

        parent(): DayPilot.Row;

        remove(): void;

        collapse(): void;

        column(i: number): RowHeaderColumn;

        expand(): void;

        removeClass(className: string): void;

        toggle(): void;
    }

    class RenderRow extends Row {
        areas: AreaData[];
        backColor: string;
        contextMenu: DayPilot.Menu;
        cssClass: string;
        fontColor: string;
        horizontalAlignment: HorizontalAlignment;
        html: string;
        text: string;
        moveDisabled: boolean;
        verticalAlignment: VerticalAlignment;
        toolTip: string;
        columns: {
            areas: AreaData[];
            backColor: string;
            cssClass: string;
            horizontalAlignment: HorizontalAlignment;
            html: string;
            text: string;
        }[];
    }

    class Selection {
        start: DayPilot.Date;
        end: DayPilot.Date;
        resource: string;
    }

    class Export {
        toElement(): HTMLElement;

        toHtml(): string;

        toDataUri(): string;

        toBlob(): Blob;

        print(options?: any): void;

        download(filename?: string): void;

        dimensions(): { width: number, height: number };
    }

    interface CardData {
        id: CardId;
        name: string;
        text?: string;
        html?: string;
        column: ColumnId;
        swimlane?: SwimlaneId;
        barColor?: string;
    }

    interface KanbanColumnData {
        id: ColumnId;
        name: string;
        barColor?: string;
    }

    interface SwimlaneData {
        id: SwimlaneId;
        name: string;
        collapsed?: boolean;
    }

    interface RowHeaderColumn {
        html(newHtml?: string): string | void;
    }

    interface RowHeaderColumnData {
        title?: string;
        text?: string;
        html?: string;
        width?: number;
        display?: string;
        sort?: string;
    }

    interface EventGroup {
        expand(): void;

        collapse(): void;
    }

    interface RenderGroup {
        readonly count: number;
        readonly events: DayPilot.Event[];
        html: string;
    }

    interface CalendarColumnData {
        name: string;
        id?: ResourceId;
        start?: DayPilot.Date | string;
        html?: string;
        toolTip?: string;
        children?: CalendarColumnData[];
    }

    type GroupBy = "Minute" | "Hour" | "Day" | "Week" | "Month" | "Quarter" | "Year" | "Cell" | "None";
    type SortDirection = "asc" | "desc";
    type TaskType = "Task" | "Milestone" | "Group";
    type LinkType = "FinishToStart" | "FinishToFinish" | "StartToStart" | "StartToFinish";

    interface ZoomLevel {
        properties: any;

        [prop: string]: any;
    }

    interface TimelineData {
        start: DayPilot.Date | string;
        end: DayPilot.Date | string;
        width?: number;
    }

    interface TimeHeaderData {
        groupBy: GroupBy;
        format?: string;
        height?: number;
    }

    interface MenuItemData {
        action?: "CallBack" | "PostBack";
        command?: string;
        cssClass?: string;
        disabled?: boolean;
        hidden?: boolean;
        href?: string;
        icon?: string;
        image?: string;
        items?: MenuItemData[];
        onClick?: EventHandler<MenuItemClickArgs>;
        tags?: any;
        target?: string;
        text?: string;
        html?: string;
    }

    interface MenuItemClickArgs {
        readonly item: MenuItemData;
        readonly source: any;
        readonly originalEvent: MouseEvent;
        preventDefault(): void;
    }

    interface SeparatorData {
        location: DayPilot.Date | string;
        color?: string;
        layer?: "AboveEvents" | "BelowEvents";
        opacity?: number;
        width?: number;
        cssClass?: string;
    }

    interface CellArray extends Array<Cell> {
        addClass(className: string): CellArray;

        removeClass(className: string): CellArray;

        html(html: string): CellArray;

        invalidate(): CellArray;
    }

    interface Cell {
        readonly start: DayPilot.Date;
        readonly end: DayPilot.Date;
        readonly resource: ResourceId;
        readonly isParent: boolean;
        readonly div: HTMLElement;
        properties: CellProperties;
        readonly x: number;
        readonly y: number;
        readonly displayY: number;
        readonly grid: GridId;
        // cssClass: string;

        update(): void;

        utilization(name?: string): number;

        events(): DayPilot.Event[];
    }

    interface CellProperties {
        areas: AreaData[];
        backColor: string;
        backImage: string;
        backRepeat: string;
        business: boolean;
        cssClass: string;
        disabled: boolean;
        html: string;
    }

    type GridId = "top" | "main" | "bottom";

    interface EventDataShort {
        start: string | DayPilot.Date;
        end: string | DayPilot.Date;
    }

    interface EventData {
        start: string | DayPilot.Date;
        end: string | DayPilot.Date;
        id: EventId;
        text: string;
        resource?: ResourceId;

        areas?: AreaData[];
        backColor?: string;
        backImage?: string;
        backRepeat?: string;
        borderColor?: string;
        bubbleHtml?: string;
        clickDisabled?: boolean;
        contextMenu?: DayPilot.Menu;
        cssClass?: string;
        deleteDisabled?: boolean;
        doubleClickDisabled?: boolean;
        fontColor?: string;
        hidden?: boolean;
        html?: string;
        line?: "dedicated" | number;
        moveDisabled?: boolean;
        recurrent?: boolean;
        recurrentMasterId?: boolean;
        resizeDisabled?: boolean;
        rightClickDisabled?: boolean;
        sort?: string[];
        tags?: any;
        toolTip?: string;

        // scheduler
        barBackColor?: string;
        barColor?: string;
        barHidden?: boolean;
        complete?: number;
        container?: number | string;
        height?: number;
        htmlLeft?: string;
        htmlRight?: string;
        moveVDisabled?: boolean;
        moveHDisabled?: boolean;
        versions?: VersionData[];

        // calendar
        allday?: boolean;
    }

    interface VersionData {
        start: string | DayPilot.Date;
        end: string | DayPilot.Date;
        id?: EventId;
        text?: string;
        html?: string;
        areas?: AreaData[];
        backColor?: string;
        fontColor?: string;
        borderColor?: string;
        backImage?: string;
        backRepeat?: string;
        complete?: number;
        barColor?: string;
        barBackColor?: string;
        barImageUrl?: string;
        barHidden?: boolean;
        htmlRight?: string;
        htmlLeft?: string;
        cssClass?: string;
        toolTip?: string;
    }

    interface LinkData {
        from: string;
        to: string;
        id?: string;
        type?: LinkType;
        width?: number;
        color?: string;
        style?: string;
        cssClass?: string;
        layer?: "Above" | "Below";
    }

    interface TaskData {
        id: TaskId;
        text: string;
        start: DayPilot.Date | string;
        end?: DayPilot.Date | string;
        type?: TaskType;
        complete?: number;
        children?: TaskData[];
        tags?: any;

        box?: {
            areas?: AreaData[];
            backColor?: string;
            backImage?: string;
            backRepeat?: string;
            barBackColor?: string;
            barColor?: string;
            barHidden?: boolean;
            bubbleHtml?: string;
            clickDisabled?: boolean;
            contextMenu?: DayPilot.Menu;
            cssClass?: string;
            doubleClickDisabled?: boolean;
            html?: string;
            htmlLeft?: string;
            htmlRight?: string;
            moveDisabled?: boolean;
            resizeDisabled?: boolean;
            rightClickDisabled?: boolean;
            toolTip?: string;
        };

        row?: {
            areas?: AreaData[];
            backColor?: string;
            collapsed?: boolean;
            contextMenu?: DayPilot.Menu;
            cssClass?: string;
            hidden?: boolean;
            html?: string;
            marginBottom?: number;
            minHeight?: number;
            toolTip?: string;
        };
    }

    interface ResourceData {
        id: ResourceId;
        name?: string;

        areas?: AreaData[];
        ariaLabel?: string;
        backColor?: string;
        bubbleHtml?: string;
        fontColor?: string;
        cellsAutoUpdated?: boolean;
        cellsDisabled?: boolean;
        children?: ResourceData[];
        columns?: { text?: string, html?: string; cssClass?: string; backColor?: string }[];
        cssClass?: string;
        contextMenu?: Menu;
        dynamicChildren?: boolean;
        eventHeight?: number;
        eventStackingLineHeight?: number;
        expanded?: boolean;
        frozen?: "top" | "bottom";
        html?: string;
        marginBottom?: number;
        marginTop?: number;
        minHeight?: number;
        moveDisabled?: boolean;
        tags?: any;
        toolTip?: string;

        [prop: string]: any;
    }

    interface AreaData {
        action?: "Default" | "None" | "JavaScript" | "ContextMenu" | "HoverMenu" | "ResizeEnd" | "ResizeStart" | "Move" | "Bubble";
        backColor?: string;
        background?: string;
        bottom?: number;
        bubble?: Bubble;
        cssClass?: string;
        end?: Date | string;
        fontColor?: string;
        height?: number;
        horizontalAlignment?: HorizontalAlignment;
        html?: string;
        icon?: string;
        id?: AreaId;
        image?: string;
        left?: number;
        menu?: Menu | string;
        offsetX?: number;
        onClick?: (args: any) => void;
        onClicked?: (args: any) => void;
        onMouseEnter?: (args: any) => void;
        onMouseLeave?: (args: any) => void;
        padding?: number;
        right?: number;
        start?: Date | string;
        style?: string;
        symbol?: string;
        text?: string;
        toolTip?: string;
        top?: number;
        verticalAlignment?: VerticalAlignment;
        visibility?: "Hover" | "Visible" | "TouchVisible";
        width?: number;
    }

    function guid(): string;

    interface EventHandler<T> {
        (args: T): void;
    }

    type HorizontalAlignment = "right" | "center" | "left";
    type VerticalAlignment = "top" | "center" | "bottom";
    type FontStyle = "normal" | "italic" | "bold";
    interface SchedulerViewPort {
        start: DayPilot.Date,
        end: DayPilot.Date,
        resources: ResourceId[]
    }
    type ResourceId = string | number;
    type EventId = string | number;
    type LinkId = string | number;
    type TaskId = string | number;
    type CardId = string | number;
    type ColumnId = string | number;
    type SwimlaneId = string | number;
    type AreaId = string | number;

    interface RowHeaderExportColumn {
        backColor: string;
        fontColor: string;
        fontSize: string;
        fontFamily: string;
        fontStyle: FontStyle;
        horizontalAlignment: HorizontalAlignment;
        verticalAlignment: VerticalAlignment;
        text: string;
    }

    type MouseButton = "left" | "right" | "middle";

    // modal

    export class Modal extends ModalPropsAndEvents {
        constructor(options?: ModalConfig)

        close(result?: any): void;

        closeSerialized(): void;

        showHtml(html: string | HTMLElement): void;

        showUrl(url: string): void;

        stretch(): void;
    }

    export class ModalPropsAndEvents {
        autoFocus?: boolean;
        autoStretch?: boolean;
        autoStretchFirstLoadOnly?: boolean;
        container?: HTMLElement;
        disposeOnClose?: boolean;
        dragDrop?: boolean;
        focus?: string | { id: string, value: string | number };
        height?: number;
        left?: number;
        loadingHtml?: string;
        maxHeight?: number;
        scrollWithPage?: boolean;
        theme?: string;
        top?: number;
        useIframe?: boolean;
        width?: number;
        zIndex?: number;

        onClose?: EventHandler<ModalCloseArgs>;
        onClosed?: EventHandler<ModalClosedArgs>;
        onShow?: EventHandler<ModalShowArgs>;
    }

    export namespace Modal {
        function close(): void;

        function opener(): void;

        function prompt(message: string, defaultValue?: string, options?: ModalPromptConfig): Promise<ModalClosedArgs>;

        function alert(message: string, options?: ModalAlertConfig): Promise<ModalClosedArgs>;

        function confirm(message: string, options?: ModalConfirmConfig): Promise<ModalClosedArgs>;

        function form(form?: ModalFormItem[], data?: any, options?: ModalFormConfig): Promise<ModalClosedArgs>;
    }

    export class ModalConfig extends ModalPropsAndEvents {
    }

    export class ModalAlertConfig extends ModalConfig {
        okText?: string;
    }

    export class ModalConfirmConfig extends ModalConfig {
        okText?: string;
        cancelText?: string;
    }

    export class ModalPromptConfig extends ModalConfig {
        okText?: string;
        cancelText?: string;
    }

    export class ModalFormConfig extends ModalConfig {
        okText?: string;
        cancelText?: string;
        locale?: string;
        plugins?: any;
    }

    export interface ModalCloseArgs {
        canceled: boolean;
        result: any;
        backgroundClick: boolean;

        preventDefault(): void;
    }

    export interface ModalClosedArgs {
        canceled: boolean;
        result: any;
        backgroundClick: boolean;
    }

    export interface ModalShowArgs {
        root: Node
    }


    export interface ModalFormItem {
        id?: string;
        name?: string;
        type?: "text" | "date" | "searchable" | "select" | "radio" | "checkbox" | "table" | "title" | "image" | "html" | "textarea" | "scrollable" | string;
        image?: string;
        dateFormat?: string;
        disabled?: boolean;
        cssClass?: string;
        options?: ModalFormOption[];
        children?: ModalFormItem[];
        columns?: ModalFormTableColumns[];
        onValidate?: EventHandler<ModalFormItemValidationArgs>;
        onNewRow?: EventHandler<ModalFormTableItemNewRowArgs>;
        height?: number;
        text?: string;
        html?: string;
    }

    export interface ModalFormOption {
        id: string | number;
        name?: string;
        children?: ModalFormItem[];
    }

    export interface ModalFormTableColumns {
        id: string;
        name: string;
        type?: "text" | "number" | "select";
        options?: ModalFormOption[];
    }

    export interface ModalFormItemValidationArgs {
        value: any;
        result: any;
        valid: boolean;
        message: string;
    }

    export interface ModalFormTableItemNewRowArgs {
        value: any;
        result: any;
    }

}
