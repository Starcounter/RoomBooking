template <>
struct mapper<RoomBooking::RoomBookingEvent>
{
    static M::Entity on_create()
    {
        return BookingEvent.create();
    }

    static void on_update_Room(entity root, const option<RoomBooking::Room> &value)
    {
        if (value)
            root.ensure_subject_of(ParticipantRelation, entity::from(*value));
        else
            root.ensure_not_subject_of(ParticipantRelation, Room);
    }

    static void on_update_BeginUtcDate(entity root, const datetime &value)
    {
        root.start(value != datetime::min_value() ? some(value) : none);
    }

    static void on_update_EndUtcDate(entity root, const datetime &value)
    {
        if (value)
        {
            root.end(value);
        }
    }

    static void on_update_Name(entity root, const option<string> &value)
    {
        if (value)
            root.ensure_subject_of(HaveRelation, Name.ensure_string(*value));
        else
            root.ensure_not_subject_of(HaveRelation, Name);
    }

    static void on_update_Description(entity root, const option<string> &value)
    {
        if (value)
            root.ensure_subject_of(HaveRelation, Description.ensure_string(*value));
        else
            root.ensure_not_subject_of(HaveRelation, Description);
    }

    static void on_update_WarnNotificationMinutes(entity root, const option<int> &value)
    {
        // TODO(wanderlust-ginge)
    }

    static void on_delete(entity root)
    {
        root.untrack_including_relations();
    }

    static void invalidate(change_type type, const entity &obj, const entity::changes &changes, entity::invset &inval)
    {
        const auto F = obj.features() + (changes.features_assigned() ? changes.features() : featureset{});

        if (BookingEvent.test(F))
        {
            inval(obj);
        }

        if (ParticipantRelation.test(F))
        {
            from(obj.subject()).filter(BookingEvent).for_each(inval);
            from(changes.subject_assigned() ? changes.subject() : none).filter(BookingEvent).for_each(inval);
        }

        if (Room.test(F))
        {
            from(obj).object_of(ParticipantRelation).having_subject(BookingEvent).for_each(inval);
        }

        if (HaveRelation.test(F))
        {
            from(obj.subject()).filter(BookingEvent).for_each(inval);
            from(changes.subject_assigned() ? changes.subject() : none).filter(BookingEvent).for_each(inval);
        }

        // TODO(wanderlust-ginge)
        if (Name.test(F) || Description.test(F) /* ||  WarnNotificationMinutes.test(F)  */)
        {
            from(obj).object_of(HaveRelation).having_subject(BookingEvent).for_each(inval);
        }
    }

    static bool refresh(const entity &root, RoomBooking::RoomBookingEvent::refresh_batch &changes)
    {
        if (!BookingEvent.test(root))
        {
            return false;
        }

        const auto room = from(root).subject_of(ParticipantRelation).having_object(Room).first();
        const auto name = from(root).subject_of(HaveRelation).having_object(Name).first();
        const auto description = from(root).subject_of(HaveRelation).having_object(Description).first();
        const auto beginUtcDate = root.start();
        const auto endUtcDate = root.end();

        // TODO(wanderlust-ginge)
        /*const auto warnNotificationMinutes
        changes.WarnNotificationMinutes
        */

        changes.Room(room ? some(room->to<RoomBooking::Room>()) : none);
        changes.Name(name ? name->string() : none);
        changes.Description(description ? description->string() : none);
        changes.BeginUtcDate(beginUtcDate ? *beginUtcDate : datetime::min_value());
        changes.EndUtcDate(endUtcDate ? *endUtcDate : datetime::max_value());

        return true;
    }
};