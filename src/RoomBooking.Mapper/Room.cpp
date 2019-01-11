template <>
struct mapper<RoomBooking::Room>
{
    static M::Entity on_create()
    {
        return Room.create();
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

    static void on_update_TimeZoneId(entity root, const option<string> &value)
    {
        if (value)
            root.ensure_subject_of(HaveRelation, TimeZone.ensure_string(*value));
        else
            root.ensure_not_subject_of(HaveRelation, TimeZone);
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

        if (Room.test(F))
        {
            inval(obj);
        }

        if (HaveRelation.test(F))
        {
            from(obj.subject()).filter(Room).for_each(inval);
            from(changes.subject_assigned() ? changes.subject() : none).filter(Room).for_each(inval);
        }

        // TODO(wanderlust-ginge)
        if (Name.test(F) || Description.test(F) || TimeZone.test(F) /* ||  WarnNotificationMinutes.test(F)*/)
        {
            from(obj).object_of(HaveRelation).having_subject(Room).for_each(inval);
        }
    }

    static bool refresh(const entity &root, RoomBooking::Room::refresh_batch &changes)
    {
        if (!Room.test(root))
        {
            return false;
        }

        const auto name = from(root).subject_of(HaveRelation).having_object(Name).first();
        const auto description = from(root).subject_of(HaveRelation).having_object(Description).first();
        const auto timeZoneId = from(root).subject_of(HaveRelation).having_object(TimeZone).first();
        // TODO(wanderlust-ginge)
        /*const auto warnNotificationMinutes
        changes.WarnNotificationMinutes*/

        changes.Name(name ? name->string() : none);
        changes.Description(description ? description->string() : none);
        changes.TimeZoneId(timeZoneId ? timeZoneId->string() : none);

        return true;
    }
};